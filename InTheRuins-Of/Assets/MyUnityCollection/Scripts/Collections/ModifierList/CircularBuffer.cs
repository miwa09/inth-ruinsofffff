using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System;

/// <summary>
/// Represents a first-in, first-out fixed size list of objects. 
/// </summary>
public class CircularBuffer<T> : IEnumerable<T> {

  public T this[int index] {
    get => data[head - index];
    set => data[head - index] = value;
  }

  public int Length { get => data.Length; }

  private T[] data;
  private CircularInt head;

  public CircularBuffer(int length) {
    data = new T[length];
    head = new CircularInt(0, length);

    var a = new List<int>();
    var b = a.AsReadOnly();
  }

  public void Add(T item) {
    data[++head] = item;
  }

  /// <summary> Set all elements to default value of element type </summary>
  internal void Clear() {
    for (int i = 0; i < data.Length; i++) {
      data[i] = default(T);
    }
  }

  IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
  public IEnumerator<T> GetEnumerator() {
    for (int i = 0; i < head.ceil; i++) {
      yield return data[head - i];
    }
  }

  public ReadOnlyCollection<T> AsReadOnly() => new ReadOnlyCollection<T>(this.ToArray());

  public new string ToString() => string.Join(", ", this.ToArray());

}
