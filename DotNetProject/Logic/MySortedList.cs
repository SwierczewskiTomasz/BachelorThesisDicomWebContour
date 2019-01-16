using System;
using DTOs;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataAccess;

namespace Logic
{
    public class MySortedListElement
    {
        public Vertex Key;
        public double Value;
        public MySortedListElement next;
        public MySortedListElement previous;
    }

    public class MySortedList
    {
        public static Random random = new Random();
        public MySortedListElement first = null;
        public MySortedListElement Add(Vertex key, int value)
        {
            double Value = value;

            MySortedListElement element = new MySortedListElement();
            element.Key = key;
            element.Value = Value;
            element.next = null;
            element.previous = null;

            if (first == null)
            {
                first = element;
            }
            else
            {
                MySortedListElement current = first;

                if (current.Value > Value)
                {
                    element.next = first;
                    first = element;
                }
                else if (current.next == null)
                {
                    first.next = element;
                    element.previous = first;
                }
                else
                {
                    while (current.next.Value < Value)
                    {
                        current = current.next;
                        if(current.next == null)
                        {
                            break;
                        }
                    }

                    element.next = current.next;
                    current.next = element;
                    if(element.next != null)
                        element.next.previous = element;
                }
            }

            return element;
        }

        public void Remove(MySortedListElement element)
        {
            if(element.next != null)
                element.next.previous = element.previous;
            if(element.previous != null)
                element.previous.next = element.next;
            if(element == first)
                first = element.next;
        }

        public (Vertex, int) First()
        {
            if(first == null)
                return (null, 0);
            else
            {
                MySortedListElement current = first;
                first = first.next;
                return (current.Key, (int)current.Value);
            }
        }
    }
}