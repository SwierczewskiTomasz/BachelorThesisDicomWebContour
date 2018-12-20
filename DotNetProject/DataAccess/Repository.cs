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
        void Delete(Guid guid);
        void Edit(TEntity entity);
    }

    public class ManualContourRepository : IFilesRepository<ManualContourDTO>
    {
        List<Guid> contours = new List<Guid>();

        public List<Guid> FetchAll()
        {
            return contours;
        }

        public ManualContourDTO Load(Guid guid)
        {
            string DICOMid;
            string tag;
            List<Line> lines = new List<Line>();
            int width;
            int height;

            string buffor;

            string filename = "../data/manual/" + guid.ToString() + ".csv";
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

            Line line = new Line();
            line.pixels = new List<Point>();

            buffor = sr.ReadLine();
            List<int> points = buffor.Split(',').Select(s => int.Parse(s)).ToList();
            int i = 0;

            while (i + 1 < points.Count)
                line.pixels.Add(new Point(points[i++], points[i++]));

            //It the same as:
            // while(i + 1 < points.Count)
            // {
            //     line.pixels.Add(new Point(points[i], points[i+1]));
            //     i += 2;
            // }
            // But it's look more funny

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

            ManualContourDTO contour = new ManualContourDTO(guid, DICOMid, tag, lines, width, height);

            return contour;
        }

        public void Save(ManualContourDTO contour)
        {
            string filename = "../data/manual/" + contour.guid.ToString() + ".csv";
            StreamWriter sw = new StreamWriter(filename);

            sw.WriteLine(contour.guid.ToString());
            sw.WriteLine(contour.dicomid.ToString());
            sw.WriteLine(contour.tag);
            sw.WriteLine(string.Join(',', contour.lines.First().pixels.Select(s => s.x.ToString() +
             "," + s.y.ToString())));
            sw.WriteLine(contour.lines.First().brushColor);
            sw.WriteLine(contour.width);
            sw.WriteLine(contour.height);

            sw.Close();

            contours.Add(contour.guid);
        }

        public void Delete(Guid guid)
        {
            string filename = "../data/manual/" + guid.ToString() + ".csv";
            File.Delete(filename);

            contours.Remove(guid);
        }

        public void Edit(ManualContourDTO contour)
        {
            Delete(contour.guid);
            Save(contour);
        }
    }

    public class SemiAutomaticContourRepository
    {
        List<Guid> contours = new List<Guid>();

        public List<Guid> FetchAll()
        {
            return contours;
        }

        public SemiAutomaticContourDTO Load(Guid guid)
        {
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

            contours.Add(contour.guid);
        }

        public void Delete(Guid guid)
        {
            string filename = "../data/semiautomatic/" + guid.ToString() + ".csv";
            File.Delete(filename);

            contours.Remove(guid);
        }

        public void Edit(SemiAutomaticContourDTO contour)
        {
            Delete(contour.guid);
            Save(contour);
        }
    }

}
