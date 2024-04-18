using UnityEngine;
using ReadyPlayerMe;

public class Test : MonoBehaviour
{
    private async void Start()
    {
        CharactersAPI charactersAPI = new CharactersAPI();
        CharactersResponse response = await charactersAPI.Preview("661fd7dd56495909f20cb819", "top=661446cbcc77dbcde05a55b1");
        
        Debug.Log(response);
    }
}
