using UnityEngine;

public class TestEndpoint {
    public void Get() {
      Debug.Log("Get Test");
    }

    public void Get(string id) {
      Debug.Log("Get Test by id " + id);
    }
}