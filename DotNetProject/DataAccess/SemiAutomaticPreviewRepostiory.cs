using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using DTOs;

namespace DataAccess
{
    public class SemiAutomaticPreviewRepository
    {
        public SemiAutomaticPreviewDTO Load(Guid guid)
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

            string filename = "../data/preview/" + guid.ToString() + ".csv";
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
            if (sr.EndOfStream)
                throw new Exception($"Unexpected end of file {filename}");

            buffor = sr.ReadLine();
            double pixelSpacing = double.Parse(buffor);
            if (sr.EndOfStream)
                throw new Exception($"Unexpected end of file {filename}");

            sr.Close();

            SemiAutomaticPreviewDTO contour = new SemiAutomaticPreviewDTO(guid, DICOMid, tag, lines, width, height, pixelSpacing, 0);

            return contour;
        }

        public void Save(SemiAutomaticPreviewDTO contour)
        {
            using (var db = new ContourContext())
            {
                ContourEntity ce = new ContourEntity();
                ce.ContourEntityId = contour.guid;
                ce.DicomId = contour.dicomid;
                ce.Tag = contour.tag;
                ce.IsManual = false;

                db.Contours.Add(ce);
                db.SaveChanges();
            }

            string filename = "../data/preview/" + contour.guid.ToString() + ".csv";
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
            string filename = "../data/preview/" + guid.ToString() + ".csv";
            File.Delete(filename);
            return true;
        }

        public void Edit(SemiAutomaticPreviewDTO contour)
        {
            Delete(contour.guid);
            Save(contour);
        }
    }

}
