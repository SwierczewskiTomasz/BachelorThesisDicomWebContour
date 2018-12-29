using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using DTOs;

namespace DataAccess
{
    public class SemiAutomaticContourRepository
    {
        public List<Guid> FetchAll()
        {
            List<Guid> contours = new List<Guid>();
            using (var db = new ContourContext())
            {
                foreach (var c in db.Contours.Where(c => !c.IsManual))
                    contours.Add(c.ContourEntityId);
            }
            return contours;
        }

        public List<Guid> FetchByDicomId(string DicomId)
        {
            List<Guid> contours = new List<Guid>();
            using (var db = new ContourContext())
            {
                foreach (var c in db.Contours.Where(c => c.IsManual && c.DicomId == DicomId))
                    contours.Add(c.ContourEntityId);
            }
            return contours;
        }

        public SemiAutomaticContourDTO Load(Guid guid)
        {
            using (var db = new ContourContext())
            {
                if (db.Contours.Where(c => c.ContourEntityId == guid).ToList().Count == 0)
                    return null;
            }

            string DICOMid;
            string tag;
            List<LinePointsAndPixels> lines = new List<LinePointsAndPixels>();
            int width;
            int height;

            string buffor;

            string filename = "../data/semiautomatic/" + guid.ToString() + ".csv";
            StreamReader sr = new StreamReader(filename);
            if (sr.EndOfStream)
                throw new Exception($"Unexpected end of file {filename}");

            buffor = sr.ReadLine();
            if (Guid.Parse(buffor) != guid)
                throw new Exception($"Guid in file diffrent that in name of file {filename}");
            if (sr.EndOfStream)
                throw new Exception($"Unexpected end of file {filename}");

            DICOMid = sr.ReadLine();
            if (sr.EndOfStream)
                throw new Exception($"Unexpected end of file {filename}");

            tag = sr.ReadLine();
            if (sr.EndOfStream)
                throw new Exception($"Unexpected end of file {filename}");

            LinePointsAndPixels line = new LinePointsAndPixels();
            line.pixels = new List<Point>();
            line.points = new List<Point>();

            buffor = sr.ReadLine();
            List<int> points = buffor.Split(',').Select(s => int.Parse(s)).ToList();
            int i = 0;

            while (i + 1 < points.Count)
                line.points.Add(new Point(points[i++], points[i++]));

            //It the same as:
            // while(i + 1 < points.Count)
            // {
            //     line.pixels.Add(new Point(points[i], points[i+1]));
            //     i += 2;
            // }
            // But it's look more funny

            buffor = sr.ReadLine();
            points = buffor.Split(',').Select(s => int.Parse(s)).ToList();

            while (i + 1 < points.Count)
                line.pixels.Add(new Point(points[i++], points[i++]));

            line.brushColor = sr.ReadLine();
            if (sr.EndOfStream)
                throw new Exception($"Unexpected end of file {filename}");

            lines.Add(line);

            buffor = sr.ReadLine();
            width = int.Parse(buffor);
            if (sr.EndOfStream)
                throw new Exception($"Unexpected end of file {filename}");

            buffor = sr.ReadLine();
            height = int.Parse(buffor);


            sr.Close();

            SemiAutomaticContourDTO contour = new SemiAutomaticContourDTO(guid, DICOMid, tag, lines, width, height);

            return contour;
        }

        public void Save(SemiAutomaticContourDTO contour)
        {
            using (var db = new ContourContext())
            {
                ContourEntity ce = new ContourEntity();
                ce.ContourEntityId = contour.guid;
                ce.DicomId = contour.dicomid;
                ce.Tag = contour.tag;
                ce.IsManual = false;
#warning "Tak tego nie powinno się robić! Do poprawy"
                // ce.UserId = Guid.Empty;
                // ce.User = null;

                db.Contours.Add(ce);
                db.SaveChanges();
            }

            string filename = "../data/semiautomatic/" + contour.guid.ToString() + ".csv";
            StreamWriter sw = new StreamWriter(filename);

            sw.WriteLine(contour.guid.ToString());
            sw.WriteLine(contour.dicomid.ToString());
            sw.WriteLine(contour.tag);
            sw.WriteLine(string.Join(',', contour.lines.First().points.Select(s => s.x.ToString() +
             "," + s.y.ToString())));
            sw.WriteLine(string.Join(',', contour.lines.First().pixels.Select(s => s.x.ToString() +
             "," + s.y.ToString())));
            sw.WriteLine(contour.lines.First().brushColor);
            sw.WriteLine(contour.width);
            sw.WriteLine(contour.height);

            sw.Close();
        }

        public bool Delete(Guid guid)
        {
            using (var db = new ContourContext())
            {
                ContourEntity ce = db.Contours.Single(c => c.ContourEntityId == guid);
                if (ce.IsManual)
                    return false;
                db.Contours.Remove(ce);
                db.SaveChanges();
            }
            string filename = "../data/semiautomatic/" + guid.ToString() + ".csv";
            File.Delete(filename);
            return true;
        }

        public void Edit(SemiAutomaticContourDTO contour)
        {
            Delete(contour.guid);
            Save(contour);
        }
    }

}
