using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using DTOs;

namespace DataAccess
{
    public interface IFilesRepository<TEntity> where TEntity : class
    {
        TEntity Load(Guid guid);
        void Save(TEntity entity);
        bool Delete(Guid guid);
        bool Edit(TEntity entity);
    }

    public class ManualContourRepository : IFilesRepository<ManualContourDTO>
    {
        public List<Guid> FetchAll()
        {
            List<Guid> contours = new List<Guid>();
            using (var db = new ContourContext())
            {
                foreach (var c in db.Contours.Where(c => c.IsManual))
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

        public ManualContourDTO Load(Guid guid)
        {
            using (var db = new ContourContext())
            {
                if (db.Contours.Where(c => c.ContourEntityId == guid).ToList().Count == 0)
                    return null;
            }

            string DICOMid;
            string tag;
            List<Line> lines = new List<Line>();
            int width;
            int height;

            string buffor;

            string filename = "../data/manual/" + guid.ToString() + ".csv";

            StreamReader sr = null;
            try
            {
                sr = new StreamReader(filename);
            }
            catch (Exception)
            {
                return null;
            }

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

            Line line = new Line();
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

            line.brushColor = sr.ReadLine();
            line.brushRadius = 0;
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

            StatisticsResult statisticsResult = new StatisticsResult();

            buffor = sr.ReadLine();
            List<int> list = buffor.Split(',').Select(s => int.Parse(s)).ToList();
            statisticsResult.CenterOfMass = new Point(list[0], list[1]);
            if (sr.EndOfStream)
                throw new Exception($"Unexpected end of file {filename}");

            buffor = sr.ReadLine();
            statisticsResult.Histogram = buffor.Split(',').Select(s => int.Parse(s)).ToArray();
            if (sr.EndOfStream)
                throw new Exception($"Unexpected end of file {filename}");

            buffor = sr.ReadLine();
            statisticsResult.HistogramMin = int.Parse(buffor);
            if (sr.EndOfStream)
                throw new Exception($"Unexpected end of file {filename}");

            buffor = sr.ReadLine();
            statisticsResult.HistogramMax = int.Parse(buffor);
            if (sr.EndOfStream)
                throw new Exception($"Unexpected end of file {filename}");

            buffor = sr.ReadLine();
            statisticsResult.HistogramMean = double.Parse(buffor);
            if (sr.EndOfStream)
                throw new Exception($"Unexpected end of file {filename}");

            buffor = sr.ReadLine();
            statisticsResult.Area = double.Parse(buffor);
            if (sr.EndOfStream)
                throw new Exception($"Unexpected end of file {filename}");

            buffor = sr.ReadLine();
            statisticsResult.Permieter = double.Parse(buffor);
            if (sr.EndOfStream)
                throw new Exception($"Unexpected end of file {filename}");

            buffor = sr.ReadLine();
            statisticsResult.NumberOfPixelsInsideContour = int.Parse(buffor);
            if (sr.EndOfStream)
                throw new Exception($"Unexpected end of file {filename}");

            buffor = sr.ReadLine();
            statisticsResult.NumberOfPixelsOfContour = int.Parse(buffor);
            if (sr.EndOfStream)
                throw new Exception($"Unexpected end of file {filename}");

            buffor = sr.ReadLine();
            points = buffor.Split(',').Select(s => int.Parse(s)).ToList();
            i = 0;
            List<Point> centralPoints = new List<Point>();
            while (i + 1 < points.Count)
                centralPoints.Add(new Point(points[i++], points[i++]));


            sr.Close();

            ManualContourDTO contour = new ManualContourDTO(guid, DICOMid, tag, lines, width, height, statisticsResult, centralPoints, pixelSpacing);

            return contour;
        }

        public void Save(ManualContourDTO contour)
        {
            using (var db = new ContourContext())
            {
                ContourEntity ce = new ContourEntity();
                ce.ContourEntityId = contour.guid;
                ce.DicomId = contour.dicomid;
                ce.Tag = contour.tag;
                ce.IsManual = true;

                db.Contours.Add(ce);
                db.SaveChanges();
            }

            string filename = "../data/manual/" + contour.guid.ToString() + ".csv";
            StreamWriter sw = new StreamWriter(filename);

            sw.WriteLine(contour.guid.ToString());
            sw.WriteLine(contour.dicomid.ToString());
            sw.WriteLine(contour.tag);
            sw.WriteLine(string.Join(',', contour.lines.First().points.Select(s => s.x.ToString() +
             "," + s.y.ToString())));
            sw.WriteLine(contour.lines.First().brushColor);
            sw.WriteLine(contour.width);
            sw.WriteLine(contour.height);
            sw.WriteLine(contour.pixelSpacing);
            sw.WriteLine(contour.statistics.CenterOfMass.x + "," + contour.statistics.CenterOfMass.y);

            if (contour.statistics.Histogram == null)
                sw.WriteLine();
            else
                sw.WriteLine(string.Join(',', contour.statistics.Histogram));
            sw.WriteLine(contour.statistics.HistogramMin);
            sw.WriteLine(contour.statistics.HistogramMax);
            sw.WriteLine(contour.statistics.HistogramMean);
            sw.WriteLine(contour.statistics.Area);
            sw.WriteLine(contour.statistics.Permieter);
            sw.WriteLine(contour.statistics.NumberOfPixelsInsideContour);
            sw.WriteLine(contour.statistics.NumberOfPixelsOfContour);
            sw.WriteLine(string.Join(',', contour.centralPoints.Select(s => s.x.ToString() +
            "," + s.y.ToString())));

            sw.Close();
        }

        public bool Delete(Guid guid)
        {
            using (var db = new ContourContext())
            {
                ContourEntity ce = null;
                try
                {
                    ce = db.Contours.Single(c => c.ContourEntityId == guid);
                }
                catch (Exception)
                {
                    return false;
                }
                if (ce == null)
                    return false;
                if (!ce.IsManual)
                    return false;
                db.Contours.Remove(ce);
                db.SaveChanges();
            }
            string filename = "../data/manual/" + guid.ToString() + ".csv";
            File.Delete(filename);
            return true;
        }

        public bool Edit(ManualContourDTO contour)
        {
            if (Delete(contour.guid))
            {
                Save(contour);
                return true;
            }
            return false;
        }
    }
}
