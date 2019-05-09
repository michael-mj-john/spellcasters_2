using UnityEngine;
using System.Collections;
using System;
using System.Text;

public class SphinxTest : MonoBehaviour {
	string str;
	[SerializeField]
	GameObject fire;
	[SerializeField]
	GameObject shield;
	[SerializeField]
	GameObject heal;
	[SerializeField]
	Transform spawn;

	// Use this for initialization
	void Start () {
		UnitySphinx.Init ();
		UnitySphinx.Run ();
        UnitySphinx.SetSearchModel(UnitySphinx.SearchModel.jsgf);
    }

	void Update()
	{
		str = UnitySphinx.DequeueString ();
        /*
		if (UnitySphinx.GetSearchModel() == "kws")
		{
			print ("listening for keyword");
			if (str != "") {
				UnitySphinx.SetSearchModel (UnitySphinx.SearchModel.jsgf);
				print (str);
			}
		}
		else if (UnitySphinx.GetSearchModel() == "jsgf")
		{
			print ("listening for order");
			if (str != "") 
			{
				char[] delimChars = { ' ' };
				string[] cmd = str.Split (delimChars);
				int numAnimals = interpretNum(cmd [0]);
				GameObject animal = interpretAnimal (cmd [1]);
				for (int i=0; i < numAnimals; i++) {
					Vector3 randPos = 
						new Vector3 (spawn.position.x + UnityEngine.Random.Range (-0.1f, 0.1f), 
							spawn.position.y + UnityEngine.Random.Range (-0.1f, 0.1f), 
							spawn.position.z + UnityEngine.Random.Range (-0.1f, 0.1f));
					Instantiate (animal, randPos, spawn.rotation);
				}
				UnitySphinx.SetSearchModel (UnitySphinx.SearchModel.kws);
			}
		}*/
        if (str != "")
        {
            char[] delimChars = { ' ' };
            string[] cmd = str.Split(delimChars);
            //int numAnimals = interpretNum(cmd[0]);
            GameObject animal = interpretAnimal(str);
            if (animal == null)
                return;
            //for (int i = 0; i < numAnimals; i++)
            //{
                Vector3 randPos =
                    new Vector3(spawn.position.x + UnityEngine.Random.Range(-0.1f, 0.1f),
                        spawn.position.y + UnityEngine.Random.Range(-0.1f, 0.1f),
                        spawn.position.z + UnityEngine.Random.Range(-0.1f, 0.1f));
                Instantiate(animal, randPos, spawn.rotation);
            //}
        }

    }

    GameObject interpretAnimal(string animal)
	{
		GameObject a = null;
        if (animal == "fire blast")
            a = fire;
        else if (animal == "shield block")
            a = shield;
        else if (animal == "life heal")
            a = heal;
		return a;
	}
}