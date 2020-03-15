using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

[DefaultExecutionOrder(1000)]
public class PositionHistory : MonoBehaviour, IEnumerable<Vector3> {

  public Vector3 this[int index] { get => positions[index]; }
  public int Length { get => positions.Length; private set => positions.Length = value; }
  public int Size { get => positions.Length; private set => positions.Length = value; }
  IEnumerator IEnumerable.GetEnumerator() => positions.GetEnumerator();
  public IEnumerator<Vector3> GetEnumerator() => positions.GetEnumerator();

  private CircularBuffer<Vector3> positions = new CircularBuffer<Vector3>(2);

  void LateUpdate() {
    positions.Add(transform.position);
  }

  /// <summary>
  /// Sets the size of the history to length if it would increase it. 
  /// </summary>
  /// <returns> Resulting length of history</returns>
  public int SetMinSize(int length) {
    if (length <= Length) return Length;
    Length = length;
    return Length;
  }
}