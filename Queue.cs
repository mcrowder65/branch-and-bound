using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TSP
{
   /* public class Queue
    {
        List<State> heap;
        Dictionary<PointF, int> pointerArray;
        public Queue()
        {
            heap = new List<State>();
            pointerArray = new Dictionary<PointF, int>();
        }
        public Dictionary<PointF, int> getPointerArray()
        {
            return pointerArray;
        }
        public List<State> getHeap()
        {
            return heap;
        }
        public State getNode(int index)
        {
            return heap[index];
        }
        public int size()
        {
            return heap.Count;
        }
        public void insert(State x)
        {
            heap.Add(x);
            heap[heap.Count - 1].setIndex(heap.Count - 1);
            pointerArray[heap[heap.Count - 1].getPoint()] = heap.Count - 1;
            bubbleUp(x, heap.Count - 1); //O(logn)
        }
        public void decreaseKey(State x)
        {
            int index = pointerArray[x.getPoint()];//O(1) lookup time, in a dictionary..
            if (index == -1)
                return;
            bubbleUp(x, Convert.ToDouble(index));
        }

        public void makeHeap(List<PointF> points, PointF startingNode, string method) //O(n)
        {
            heap.Add(new Node(0, startingNode, 0));
            pointerArray[startingNode] = 0;
            if (method == "allPath")
            {
                for (int i = 1; i < points.Count; i++)
                {//O(n), looks at all elements in points
                    heap.Add(new Node(Int32.MaxValue, points[i], i));
                    pointerArray[points[i]] = i;
                }

            }
        }
        public void bubbleUp(Node x, double i)//O(logn)
        {
            int parent = Convert.ToInt32(Math.Floor((i - 1) / 2));
            int index = Convert.ToInt32(i); // i
            heap[index] = x;

            while (index != 0 && heap[parent].getDistance() > heap[index].getDistance())
            {//if parent is greater than child, swap it until at root -> worst case O(logn)
                Node temp = heap[index];
                heap[index] = heap[parent];
                heap[index].setIndex(index);
                pointerArray[heap[index].getPoint()] = index;

                heap[parent] = temp;
                heap[parent].setIndex(parent);
                pointerArray[heap[parent].getPoint()] = parent;

                //set parent back to child
                index = parent;
                parent = Convert.ToInt32(Math.Floor(Convert.ToDouble(index - 1) / 2));
            }

        }
        public Node deleteMin() //O(logn)
        {
            if (heap.Count == 0)
                return null;

            Node x = heap[0];
            heap[0] = heap[heap.Count - 1];
            heap[0].setIndex(0);
            pointerArray[heap[0].getPoint()] = 0;
            if (heap.Count != 1)
            {
                pointerArray[heap[heap.Count - 1].getPoint()] = -1;
                heap.RemoveAt(heap.Count - 1);
            }
            siftDown(heap[0], 0); //O(logn)
            return x;
        }
        public void siftDown(Node x, int i)//O(logn)
        {
            int min = minChild(i);
            while (min != 0 && heap[min].getDistance() < heap[x.getIndex()].getDistance())//O(logn), because you don't look at every element
            {
                Node temp = heap[i];
                heap[i] = heap[min];
                heap[i].setIndex(i);
                heap[min] = temp;
                heap[min].setIndex(min);
                i = min;

                min = minChild(i); //O(1)
            }
        }

        public int minChild(int i)
        {//O(1), only contains comparisons
            if (heap.Count == 1)
                return 0;
            if (heap.Count == 2)
            {
                if (heap[0].getDistance() > heap[1].getDistance())
                    return 1;
            }
            if (2 * i + 2 > heap.Count - 1)
            {
                return i;
            }
            if (heap[i * 2 + 1].getDistance() < heap[i * 2 + 2].getDistance())
            {
                return i * 2 + 1;
            }
            return i * 2 + 2;
        }
    }*/
}
