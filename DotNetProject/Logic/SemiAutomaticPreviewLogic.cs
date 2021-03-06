using System;
using DTOs;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataAccess;

namespace Logic
{
    public class SemiAutomaticPreviewLogic
    {
        private readonly SemiAutomaticPreviewRepository repository = new SemiAutomaticPreviewRepository();

        public SemiAutomaticPreviewDTO Get(Guid guid)
        {
            SemiAutomaticPreviewDTO result = null;
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

        public SemiAutomaticPreviewDTO Add(SemiAutomaticPreviewDTO contour)
        {
            SemiAutomaticPreviewDTO result = SemiAutomatic.Default(contour);
            repository.Save(result);
            return result;
        }

        public bool Delete(Guid guid)
        {
            return repository.Delete(guid);
        }

        public (bool, SemiAutomaticPreviewDTO) Edit(SemiAutomaticPreviewDTO contour)
        {
            SemiAutomaticPreviewDTO old = repository.Load(contour.guid);

            if (old == null)
            {
                return (false, null);
            }

            List<Point> newListOfPoints = new List<Point>();

            int i = 0;
            int j = 0;
            int countOld = old.lines.First().points.Count;
            int countNew = contour.lines.First().points.Count;

            Point currentInOld = old.lines.First().points[i];
            Point currentInNew = contour.lines.First().points[j];

            while (i < countOld)
            {
                currentInOld = old.lines.First().points[i];
                currentInNew = contour.lines.First().points[j];
                while (currentInOld.x != currentInNew.x || currentInOld.y != currentInNew.y)
                {
                    i++;
                    if(i == countOld)
                        break;
                    currentInOld = old.lines.First().points[i];
                }
                newListOfPoints.Add(currentInOld);
                i++;
                j++;
                if (j == countNew)
                    break;
            }
            while (j < countNew)
            {
                currentInNew = contour.lines.First().points[j];
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

                    double dy = Math.Abs(point2.y - point1.y);
                    double dx = Math.Abs(point2.x - point1.x);

                    bool inside = true;

                    if (currentInNew.x > Math.Max(point1.x, point2.x) + (dx + dy) / 2)
                        inside = false;
                    if (currentInNew.x < Math.Min(point1.x, point2.x) - (dx + dy) / 2)
                        inside = false;
                    if (currentInNew.y > Math.Max(point1.y, point2.y) + (dy + dx) / 2)
                        inside = false;
                    if (currentInNew.y < Math.Min(point1.y, point2.y) - (dy + dx) / 2)
                        inside = false;

                    if (distance < minDistance && inside)
                    {
                        minDistance = distance;
                        index = k + 1;
                    }
                }

                newListOfPoints.Insert(index, currentInNew);
                j++;
            }

            contour.lines.First().points = new List<Point>(newListOfPoints);

            List<LinePointsAndPixels> list = new List<LinePointsAndPixels>();
            LinePointsAndPixels line = new LinePointsAndPixels();
            line.points = new List<Point>(newListOfPoints);
            line.pixels = old.lines.First().pixels;
            line.brushColor = contour.lines.First().brushColor;
            list.Add(line);

            SemiAutomaticPreviewDTO contourPointsDTO = new SemiAutomaticPreviewDTO(contour.guid, contour.dicomid, contour.tag, list, contour.width, contour.height,
                contour.pixelSpacing, contour.disablePreviewCalculations);
            SemiAutomaticPreviewDTO result = contourPointsDTO;
            if (!contour.disablePreviewCalculations)
            {
                result = SemiAutomatic.Default(contourPointsDTO);
            }

            if (repository.Edit(result))
            {
                return (true, result);
            }
            else
            {
                return (false, result);
            }
        }
    }
}
