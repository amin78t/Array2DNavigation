using System;
using System.Collections.Generic;
namespace Array2DNavigation
{
    public class CustomList<T>
    {
        #region Variables
        private const int defaultCapacity = 4;
        private T[] array;
        private int lastItemIndex;
        private IComparer<T> comparer;
        #endregion

        #region Properties
        public int Count
        {
            get
            {
                return lastItemIndex + 1;
            }
        }
        #endregion

        #region Constructors
        public CustomList(Comparison<T> comparison, int capacity = defaultCapacity)
        {
            var comparer = Comparer<T>.Create(comparison);
            array = new T[capacity];
            lastItemIndex = -1;
            this.comparer = comparer;
        }
        #endregion

        #region Functions
        public void Add(T item)
        {
            if (lastItemIndex == array.Length - 1)
            {
                mResize();
            }

            lastItemIndex++;
            array[lastItemIndex] = item;

            mUp(lastItemIndex);
        }
        public T Remove()
        {
            if (lastItemIndex == -1)
            {
                throw new InvalidOperationException("The heap is empty");
            }

            T removedItem = array[0];
            array[0] = array[lastItemIndex];
            lastItemIndex--;

            mDown(0);

            return removedItem;
        }
        public T Peek()
        {
            if (lastItemIndex == -1)
            {
                throw new InvalidOperationException("The heap is empty");
            }

            return array[0];
        }
        public void Clear()
        {
            lastItemIndex = -1;
        }
        private void mUp(int index)
        {
            if (index == 0)
            {
                return;
            }

            int childIndex = index;
            int parentIndex = (index - 1) / 2;

            if (comparer.Compare(array[childIndex], array[parentIndex]) < 0)
            {
                T temp = array[childIndex];
                array[childIndex] = array[parentIndex];
                array[parentIndex] = temp;

                mUp(parentIndex);
            }
        }
        private void mDown(int index)
        {
            int leftChildIndex = index * 2 + 1;
            int rightChildIndex = index * 2 + 2;
            int smallestItemIndex = index;

            if (leftChildIndex <= lastItemIndex &&
                comparer.Compare(array[leftChildIndex], array[smallestItemIndex]) < 0)
            {
                smallestItemIndex = leftChildIndex;
            }

            if (rightChildIndex <= lastItemIndex &&
                comparer.Compare(array[rightChildIndex], array[smallestItemIndex]) < 0)
            {
                smallestItemIndex = rightChildIndex;
            }

            if (smallestItemIndex != index)
            {
                T temp = array[index];
                array[index] = array[smallestItemIndex];
                array[smallestItemIndex] = temp;

                mDown(smallestItemIndex);
            }
        }
        private void mResize()
        {
            T[] newArr = new T[array.Length * 2];
            for (int i = 0; i < array.Length; i++)
            {
                newArr[i] = array[i];
            }

            array = newArr;
        }
        #endregion
    }
}