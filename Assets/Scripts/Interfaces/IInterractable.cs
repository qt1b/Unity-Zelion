using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// interractable should have a trigger, and react when the interract button is touched
public interface IInterractable
{
    // Start is called before the first frame update
    void Start() { }

    // Update is called once per frame
    void Update() { }

    void Interract() {}
}
