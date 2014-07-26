using UnityEngine;
using System.Collections;

public class DiceSwipeControl : MonoBehaviour
{
		// Static Instance of the Dice
		public static DiceSwipeControl Instance;
    #region Public Variables
		// Orignal Dice
		public GameObject orignalDice;
		//dice resultant number..
		public int diceCount;
		//dice play view camera...
		public Camera dicePlayCam;
		//Can Throw Dice
		public bool isDiceThrowable = true;
        public GUIText gui;
        public Transform diceCarrom;
    #endregion

    #region Private Varibles
		private GameObject diceClone;
		private Vector3 initPos;
		private float initXpose;
		private float timeRate;
		// To Save Camera Postion
		private Vector3 currentCampPos;
		Vector3 objectPos;
		internal float diceThrowInit;
    #endregion

		void Awake ()
		{
				Instance = this;

		}

		void Start ()
		{
				generateDice ();
		}

		void Update ()
		{
				if (isDiceThrowable)
                {
					if (Input.GetMouseButtonDown (0)) 
                    {
							initPos = Input.mousePosition;
							initXpose = dicePlayCam.ScreenToViewportPoint (Input.mousePosition).x;
					}
					Vector3 currentPos = Input.mousePosition;
					currentPos.z = 25f;
                    Vector3 newPos = dicePlayCam.ScreenToWorldPoint (new Vector3(currentPos.x,currentPos.y,Mathf.Clamp(currentPos.y/10,5,70)));
                    newPos.y = Mathf.Clamp(newPos.y, -114.5f, 100);
					newPos = dicePlayCam.ScreenToWorldPoint (currentPos);
					if (Input.GetMouseButtonUp (0)) 
                    {
						initPos = dicePlayCam.ScreenToWorldPoint (initPos);
		
						enableTheDice ();
						addForce (newPos);
						isDiceThrowable = false;
								
						StartCoroutine (getDiceCount ());
					}
                }
		}

		void addForce (Vector3 lastPos)
		{
                diceClone.rigidbody.AddTorque(Vector3.Cross(lastPos, initPos) * 1000, ForceMode.Impulse);
				lastPos.y += 12;
				diceClone.rigidbody.AddForce (((lastPos - initPos).normalized) * (Vector3.Distance (lastPos, initPos)) * 30 * diceClone.rigidbody.mass);
		}

		void enableTheDice ()
		{		
				diceClone.transform.rotation = Quaternion.Euler (Random.Range (0, 180), Random.Range (0, 180), Random.Range (0, 180));
				diceThrowInit = 0;
		}

		void generateDice ()
		{
				diceClone = Instantiate (orignalDice, dicePlayCam.transform.position, Quaternion.Euler (Random.Range (0, 180), Random.Range (0, 180), Random.Range (0, 180))) as GameObject;	
		}
    #region Coroutines

		IEnumerator getDiceCount ()
		{
				//Time.timeScale = 1.3f;	
				currentCampPos = dicePlayCam.transform.position;
				//wait fore dice to stop...
				yield return new WaitForSeconds (1.0f);
				while (diceClone.rigidbody.velocity.magnitude > 0.05f) {
						yield return 0;
				}

				Time.timeScale = 0.2f;
				timeRate = 0.001f;
				float startTime = Time.time;
				Vector3 risePos = dicePlayCam.transform.position;
				Vector3 setPos = new Vector3 (diceCarrom.position.x, diceClone.transform.position.y + 25f, diceCarrom.position.z);
				float speed = 0.18f;
				float fracComplete = 0;

				while (Vector3.Distance(dicePlayCam.transform.position,setPos)>0.5f) 
                {
						Vector3 center = (risePos + setPos) * 0.5f;
						center -= new Vector3 (0, 2, -1);
						Vector3 riseRelCenter = risePos - center;
						Vector3 setRelCenter = setPos - center;
				
						if (fracComplete > 0.85f && fracComplete < 1f) {
								speed += Time.deltaTime * 0.3f;
								Time.timeScale -= Time.deltaTime * 4f;
						} 
						dicePlayCam.transform.position = Vector3.Slerp (riseRelCenter, setRelCenter, fracComplete);
						dicePlayCam.transform.position += center;
						dicePlayCam.transform.LookAt (diceCarrom);
						fracComplete = (Time.time - startTime) / speed;
						yield return 0;
				}

				Time.timeScale = 1.0f;
                Dice.Instance.GetDiceCount();
                gui.text = "Dice Count : " + Dice.Instance.GetDiceCount();
				yield return new WaitForSeconds (5f);

				Dice.Instance.diceCount = 0;
				Dice.Instance.GetDiceCount ();
				diceCount = Dice.Instance.GetDiceCount ();
                gui.text = "Dice Count : " + diceCount;
				diceClone.rigidbody.constraints = RigidbodyConstraints.None;
                Application.LoadLevel(0);
		}

    #endregion
}
