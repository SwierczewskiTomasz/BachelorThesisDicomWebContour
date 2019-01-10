using System;
using DTOs;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataAccess;

namespace Logic
{
    public class SemiAutomaticContourLogic
    {
        private readonly SemiAutomaticContourRepository repository = new SemiAutomaticContourRepository();

        public List<SemiAutomaticContourDTO> FetchAllToDTOs()
        {
            List<SemiAutomaticContourDTO> contours = new List<SemiAutomaticContourDTO>();
            foreach (Guid guid in repository.FetchAll())
            {
                contours.Add(repository.Load(guid));
            }
            return contours;
        }

        public List<SemiAutomaticContourDTO> FetchByDicomIdToDTOs(string dicomid)
        {
            List<SemiAutomaticContourDTO> contours = new List<SemiAutomaticContourDTO>();
            foreach (Guid guid in repository.FetchByDicomId(dicomid))
            {
                contours.Add(repository.Load(guid));
            }
            return contours;
        }

        public List<Guid> FetchAll() => repository.FetchAll();

        public List<Guid> FetchByDicomId(string dicomid) => repository.FetchByDicomId(dicomid);


        public SemiAutomaticContourDTO Get(Guid guid)
        {
            SemiAutomaticContourDTO result = null;
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

        public SemiAutomaticContourDTO Add(SemiAutomaticPointsDTO contour)
        {
            SemiAutomaticContourDTO result = SemiAutomatic.Default(contour);
            repository.Save(result);
            return result;
        }

        public bool Delete(Guid guid)
        {
            return repository.Delete(guid);
        }

        public void Edit(SemiAutomaticContourDTO contour)
        {
            SemiAutomaticContourDTO old = repository.Load(contour.guid);

            List<Point> newListOfPoints = new List<Point>();

            int i = 0;
            int j = 0;
            int countOld = old.lines.First().points.Count;
            int countNew = contour.lines.First().points.Count;

            Point currentInOld = old.lines.First().points[i];
            Point currentInNew = contour.lines.First().points[j];

            while (i < countOld)
            {
                while (currentInOld.x != currentInNew.x || currentInOld.y != currentInNew.y)
                {
                    i++;
                    currentInOld = old.lines.First().points[i];
                }
                newListOfPoints.Add(currentInOld);
                i++;
                j++;
                if (j == countNew)
                    break;
                currentInOld = old.lines.First().points[i];
                currentInNew = contour.lines.First().points[j];
            }
            while (j < countNew)
            {
                double minDistance = double.MaxValue;
                int index = 0;

                for (int k = 0; k < newListOfPoints.Count; k++)
                {
                    Point point1 = newListOfPoints[k];
                    Point point2 = newListOfPoints[(k + 1) % newListOfPoints.Count];

                    double A = point2.y - point1.y;
                    double B = point1.x - point2.x;
                    double C = point2.x * point1.y - point1.x * point2.y;

                    double m = Math.Sqrt(A * A + B * B);
                    double distance = Math.Abs(A * currentInNew.x + B * currentInNew.y + C) / m;

                    if(distance < minDistance)
                    {
                        minDistance = distance;
                        index = k;
                    }
                }

                newListOfPoints.Insert(index, currentInNew);
            }

            contour.lines.First().points = newListOfPoints;

            repository.Edit(contour);
        }
    }
}
