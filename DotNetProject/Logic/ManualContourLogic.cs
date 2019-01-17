using System;
using DTOs;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataAccess;

namespace Logic
{
    public class ManualContourLogic
    {
        private readonly ManualContourRepository repository = new ManualContourRepository();

        public List<ManualContourDTO> FetchAllToDTOs()
        {
            List<ManualContourDTO> contours = new List<ManualContourDTO>();
            foreach (Guid guid in repository.FetchAll())
            {
                contours.Add(repository.Load(guid));
            }
            return contours;
        }

        public List<ManualContourDTO> FetchByDicomIdToDTOs(string dicomid)
        {
            List<ManualContourDTO> contours = new List<ManualContourDTO>();
            foreach (Guid guid in repository.FetchByDicomId(dicomid))
            {
                contours.Add(repository.Load(guid));
            }
            return contours;
        }

        public List<Guid> FetchAll() => repository.FetchAll();

        public List<Guid> FetchByDicomId(string dicomid) => repository.FetchByDicomId(dicomid);

        public ManualContourDTO Get(Guid guid)
        {
            ManualContourDTO result = null;
            try
            {
                result = repository.Load(guid);
            }
            catch (Exception e)
            {
                Console.WriteLine("Catched exception: ");
                Console.WriteLine(e.ToString());
            }
            return result;
        }

        public void Add(ManualContourDTO contour)
        {
            contour = PrepareContour(contour);
            contour.statistics = Statistics.GenerateStatistics(contour);
            repository.Save(contour);
        }

        public bool Delete(Guid guid)
        {
            return repository.Delete(guid);
        }

        public void Edit(ManualContourDTO contour)
        {
            repository.Edit(contour);
        }

        public static ManualContourDTO PrepareContour(ManualContourDTO contour)
        {
            List<Point> result = new List<Point>();
            int count = contour.lines.First().points.Count;
            int i;
            for(i = 0; i < count; i++)
            {
                Point p1 = contour.lines.First().points[i];
                Point p2 = contour.lines.First().points[(i + 1) % count];
                result.Add(p1);
                result.Add(p2);
                result.AddRange(BresenhamClass.Bresenham(new List<Point>(), p1.x, p1.y, p2.x, p2.y));
            }

            contour.lines.First().points = new List<Point>(result);
            return contour;
        }
    }
}
