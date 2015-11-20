using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TSP
{
    public class Queue
    {
        List<State> heap;
        Dictionary<State, int> pointerArray;
        public Queue()
        {
            heap = new List<State>();
            pointerArray = new Dictionary<State, int>();
        }

        public Dictionary<State, int> getPointerArray()
        {
            return pointerArray;
        }
        public List<State> getHeap()
        {
            return heap;
        }
        public State getState(int index)
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
            pointerArray[heap[heap.Count - 1]] = heap.Count - 1;
            bubbleUp(x, heap.Count - 1); //O(logn)
        }
        public void decreaseKey(State x)
        {
            int index = pointerArray[x];//O(1) lookup time, in a dictionary..
            if (index == -1)
                return;
            bubbleUp(x, Convert.ToDouble(index));
        }
        public void bubbleUp(State x, double i)//O(logn)
        {
            int parent = Convert.ToInt32(Math.Floor((i - 1) / 2));
            int index = Convert.ToInt32(i); // i
            heap[index] = x;

            while (index != 0 && heap[parent].getLB() > heap[index].getLB())
            {//if parent is greater than child, swap it until at root -> worst case O(logn)
                State temp = heap[index];
                heap[index] = heap[parent];
                heap[index].setIndex(index);
                pointerArray[heap[index]] = index;

                heap[parent] = temp;
                heap[parent].setIndex(parent);
                pointerArray[heap[parent]] = parent;

                //set parent back to child
                index = parent;
                parent = Convert.ToInt32(Math.Floor(Convert.ToDouble(index - 1) / 2));
            }

        }
        public State deleteMin() //O(logn)
        {
            if (heap.Count == 0)
                return null;

            State x = heap[0];
            heap[0] = heap[heap.Count - 1];
            heap[0].setIndex(0);
            pointerArray[heap[0]] = 0;
            if (heap.Count != 1)
            {
                pointerArray[heap[heap.Count - 1]] = -1;
                heap.RemoveAt(heap.Count - 1);
            }
            siftDown(heap[0], 0); //O(logn)
            return x;
        }
        public void siftDown(State x, int i)//O(logn)
        {
            int min = minChild(i);
            while (min != 0 && heap[min].getLB() < heap[x.getIndex()].getLB())//O(logn), because you don't look at every element
            {
                State temp = heap[i];
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
                if (heap[0].getLB() > heap[1].getLB())
                    return 1;
            }
            if (2 * i + 2 > heap.Count - 1)
            {
                return i;
            }
            if (heap[i * 2 + 1].getLB() < heap[i * 2 + 2].getLB())
            {
                return i * 2 + 1;
            }
            return i * 2 + 2;
        }
    }
}
