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
            uint color;
            string tag;
            List<(int, int)> pixels = new List<(int, int)>();
            string buffor;

            string filename = "../data/" + guid.ToString() + ".csv";
            StreamReader sr = new StreamReader(filename);
            if (sr.EndOfStream)
                throw new Exception($"Unexpected end of file {filename}");

            buffor = sr.ReadLine();
            if (Guid.Parse(buffor) != guid)
                throw new Exception($"Guid in file diffrent that in name of file {filename}");
            if (sr.EndOfStream)
                throw new Exception($"Unexpected end of file {filename}");

            DICOMid  = sr.ReadLine();
            if (sr.EndOfStream)
                throw new Exception($"Unexpected end of file {filename}");

            buffor = sr.ReadLine();
            color = uint.Parse(buffor);
            if (sr.EndOfStream)
                throw new Exception($"Unexpected end of file {filename}");

            tag = sr.ReadLine();
            if (sr.EndOfStream)
                throw new Exception($"Unexpected end of file {filename}");

            buffor = sr.ReadLine();
            List<int> points = buffor.Split(',').Select(s => int.Parse(s)).ToList();
            int i = 0; 

            while(i + 1 < points.Count)
                pixels.Add((points[i++], points[i++]));

            //It the same as:
            // while(i + 1 < points.Count)
            // {
            //     pixels.Add((points[i], points[i+1]));
            //     i += 2;
            // }
            // But it's look more funny

            sr.Close();

            ManualContourDTO contour = new ManualContourDTO(guid, DICOMid, pixels, color, tag);

            return contour;
        }

        public void Save(ManualContourDTO contour)
        {
            string filename = "../data/" + contour.guid.ToString() + ".csv";
            StreamWriter sw = new StreamWriter(filename);

            sw.WriteLine(contour.guid.ToString());
            sw.WriteLine(contour.DICOMid.ToString());
            sw.WriteLine(contour.color.ToString());
            sw.WriteLine(contour.tag);
            sw.WriteLine(string.Join(',',contour.pixels.Select(s => s.Item1.ToString() + 
            "," + s.Item2.ToString())));

            sw.Close();

            contours.Add(contour.guid);
        }

        public void Delete(Guid guid)
        {
            string filename = "../data/" + guid.ToString() + ".csv";
            File.Delete(filename);

            contours.Remove(guid);
        }

        public void Edit(ManualContourDTO contour)
        {
            Delete(contour.guid);
            Save(contour);
        }
    }

}
