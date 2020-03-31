using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using MUC.Inspector;

public class AbstractChildStaticFieldTest : MonoBehaviour {
  [Button]
  public void Test() {
    print(Test<float>.container.data);
    print(Test<float>.container.data);
    print(Test<int>.container.data);
    print(Test<string>.container.data);

    print("---");

    print(TestFloat.container.data);
    print(TestInt.container.data);
    print(TestString.container.data);
    print(TestBool.container.data);
  }
}

public class Container<T> {
  public string data = Guid.NewGuid().ToString();

}

public abstract class Test<T> {
  public static Container<T> container = new Container<T>();
}


public class TestFloat : Test<float> { }
public class TestInt : Test<int> { }
public class TestString : Test<string> { }
public class TestBool : Test<bool> { }