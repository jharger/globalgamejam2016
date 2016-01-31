using UnityEngine;
using System.Collections;
using System.Collections.Generic;



namespace AudioVisualizer
{

    /// <summary>
    /// Pad waveform.
    /// Create's a speaker pad waveform. Call the SendRipple() method to send an audio ripple across the pad.
    /// </summary>
    public class PadWaveform : MonoBehaviour
    {
        
		public int audioSource = 0; // index into audioSampler audioSource array. Determines which audio source we want to sample
		public int numLines = 20; // number of lines in the pad
		public float radius = 10f; // radius of the pad
		public float maxHeight = 3f; // height or amplitude of effects
		public int updateRate = 1; //re-draw every x updates
		public PadType padType = PadType.Wave;
		public Color rippleColor = Color.white; // color of ripples
        public int rippleWidth = 3; // only applies if padType = PadType.Ripple. Number of lines in each ripple
		public LineAttributes lineAttributes; // attributes for each line, in this case, start color is the middle of the pad, and end color is the outside of the pad.
		public bool gizmos = true;
		public enum PadType{Ripple,DampWave,Wave,Bounce};

		private List<LineRenderer> lines = new List<LineRenderer>(); // list of lineRenderers that make up the pad
		private Gradient padGradient; // color gradient, startcolor to endcolor
		private int updateCounter = 0; // count updates
		float fakeTimer = 0;

  

		// Use this for initialization
		void Start () 
		{
			padGradient = PanelWaveform.GetColorGradient (lineAttributes.startColor, lineAttributes.endColor);

			for(int i = 0; i < numLines; i++)
			{
				float val = (float)i/(numLines-1);
				lines.Add(NewLine(padGradient.Evaluate(val)));
			}
			CreatePad ();

			//just in case someone changes the private var. rippleLines can't be > numLines
			if( rippleWidth > numLines)
			{
				rippleWidth = numLines;
			}
		}

		// Update is called once per frame
		void FixedUpdate () 
		{

			if(updateCounter >= updateRate) //only re-draw every 'updateRate' updates
			{
				switch(padType)
				{
				case PadType.Ripple:
					Ripple();
					break;
				case PadType.DampWave:
					DampWave();
					break;
				case PadType.Wave:
					Wave();
					break;
				case PadType.Bounce:
					Bounce();
					break;
				default:
					break;
				}
				
				updateCounter = 0;
			}
			
			updateCounter ++;
		}

		//send a ripple through the pad
		//propegationTime is how long it takes for the ripple to propegate through the entire pad	
		public void SendRipple(float propegationTime)
		{
			float[] samples = AudioSampler.instance.GetAudioSamples (audioSource,lineAttributes.lineSegments, true); // get teh sames at the time of the ripple
			StartCoroutine (RipIt (propegationTime,samples));
            //Debug.Log("Ripple");
		}

		//send a wave through the pad
		/*
		 * The basic algorithm for this is as follows:
		 * while(timer < propegationTime)
		 * {
		 * 		index through the rings. index = (timer/propegationTime)*(numRings)
		 * 		for( each ring)
		 * 		{
		 * 			-get the distance to the index
		 * 			-if we're "rippleLines" or less away from the index, we're a part of the ripple
		 * 			-if we're part of the ripple, draw the rippleSamples on this ring, damped by distance from the center.
		 * 		}
		 * }
		 * */
		IEnumerator RipIt(float propegationTime, float[] rippleSamples)
		{
			
			Vector3 firstPos = Vector3.zero; // the first position we use
			float timer = 0;
			float radiusStep = radius / (numLines-1); // distance between each ring. i.e. ring0 has radius 0*radiusStep, ring10 had radius 10*radiusStep, etc.
            float angle = 0; 
			float angleStep = 360f / lineAttributes.lineSegments;//increase the angle by this much, for every point on every line, to draw each circle.
			//Debug.Log ("Ripple from " + rippleLines + " to " + numLines);
			while(timer <= propegationTime)
			{
				//what line are we on, in the range rippleLines to numLines, based on the timer
				int lineIndex = rippleWidth + (int)((numLines-rippleWidth)*(timer/propegationTime)); 
				//Debug.Log(routineNum + " lineIndex: " + lineIndex);
                //Debug.Log("lineIndex: " + lineIndex);
				//send ripple, re-draw each ring, using audiodata, and the timer.
				for(int i = rippleWidth ; i < numLines; i++)// for each line
				{
					int distance = Mathf.Abs(i-lineIndex); // get the distance i is from our lineIndex, clamped to never be > rippleLines
					if(distance <= rippleWidth)
					{
						int invDistance = rippleWidth - distance; // 0-rippleLins, invDistance is = to rippleLines if we're on the lineIndex
						float rippleDampening = (float)invDistance/rippleWidth; // dampen height based on distance from "lineIndex". maxHeight when invDistance = rippleLines
						float distDampening = (1-(float)(i-rippleWidth)/(numLines-1)); // goes from 1 to 0. dampens the height based on distance from center of the pad
						float thisRadius = radiusStep*i; // the radius of this ring
						float heightMultiplier = maxHeight*rippleDampening*distDampening; // acount for input height*dampening
                      //  Debug.Log("line: " + i + rippleDampening);

						Color lineColor = padGradient.Evaluate(1-distDampening); // color of this line, based on pad-gradient
						Gradient lineGradient = PanelWaveform.GetColorGradient (lineColor, rippleColor); // another gradient, between this line color, and rippleColor
						//color the ripple line
						Color ripColor = lineGradient.Evaluate(rippleDampening); //color/partially color the line, based on how close it is to the line index
						lines[i].SetColors(ripColor,ripColor);

						//position each line segment
						for(int j = 0; j < lineAttributes.lineSegments-1; j++) // for each line segment
						{
							float rad = Mathf.Deg2Rad*angle; // get angle in radians
							//get x,y,z of this lineSegment using equation for a circle
							float x = Mathf.Cos(rad)*thisRadius;
							float y = rippleSamples[j]*heightMultiplier; // y value based on audio info (rippleSamples) * heightMultiplier
							float z = Mathf.Sin(rad)*thisRadius;
							Vector3 pos = this.transform.position + this.transform.right*x + this.transform.up*y + this.transform.forward*z;
							lines[i].SetPosition(j,pos);
							angle += angleStep; // increase angle by angleStep
							if(j == 0)
							{
								firstPos = pos; // track the first lineSegment position
							}
						}
						lines[i].SetPosition(lineAttributes.lineSegments-1,firstPos); // set the last pos = to the first pos.
					}
				}

				//Debug.Log(routineNum + " timer: " + timer);
				timer += Time.fixedDeltaTime;
				//timers[routineNum] = timer;
				yield return null;
			}

			//after we're done, we need to re-color the last few lines, and set their y postitions back to zero
			for(int i = numLines-rippleWidth; i < numLines; i++)
			{
				float percent = (float)i/(rippleWidth-1);
				Color lineColor = padGradient.Evaluate(percent); // color of this line, based on pad-gradient
				lines[i].SetColors(lineColor,lineColor);
				float thisRadius = radiusStep*i; // the radius of this ring
				//position each line segment
				for(int j = 0; j < lineAttributes.lineSegments-1; j++) // for each line segment
				{
					float rad = Mathf.Deg2Rad*angle; // get angle in radians
					//get x,y,z of this lineSegment using equation for a circle
					float x = Mathf.Cos(rad)*thisRadius;
					float y = 0; 
					float z = Mathf.Sin(rad)*thisRadius;
					Vector3 pos = this.transform.position + this.transform.right*x + this.transform.up*y + this.transform.forward*z;
					lines[i].SetPosition(j,pos);
					angle += angleStep; // increase angle by angleStep
					if(j == 0)
					{
						firstPos = pos; // track the first lineSegment position
					}
				}
				lines[i].SetPosition(lineAttributes.lineSegments-1,firstPos); // set the last pos = to the first pos.
			}
		}

		//display the waveform in the first 'rippleLines' rings of the audioPad
		void Ripple()
		{

			float angleStep = 360f / lineAttributes.lineSegments;
			float[] samples = AudioSampler.instance.GetAudioSamples(audioSource,lineAttributes.lineSegments, true); //take 1 sample per vertex point

			//only draw a line in the middle,

			float angle = 0; // 
			float thisRadius = radius / (numLines-1); // the radius of this ring
			Vector3 firstPos = Vector3.zero; // the first position we use
			for(int j = 0; j < lineAttributes.lineSegments-1; j++)
			{
				//place each segment around the circle, wiht the given radius
				float rad = Mathf.Deg2Rad*angle;
				float x = Mathf.Cos(rad)*thisRadius;
				float y = samples[j]*maxHeight; // y value based on audio data
				float z = Mathf.Sin(rad)*thisRadius;
				Vector3 pos = this.transform.position + this.transform.right*x + this.transform.up*y + this.transform.forward*z;
				lines[0].SetPosition(j,pos);
				angle += angleStep;
				if(j == 0)
				{
					firstPos = pos;
				}
			}
			lines[0].SetPosition(lineAttributes.lineSegments-1,firstPos); // set the last pos = to the first pos.
			
			

		}

		//display the audio across all rings
		void Wave()
		{
			float radiusStep = radius / (numLines-1);
			float angleStep = 360f / lineAttributes.lineSegments;
			for(int i = 0; i < numLines; i++)
			{
				float angle = 0; // 
				float thisRadius = radiusStep*i; // the radius of this ring
				float[] samples = AudioSampler.instance.GetAudioSamples(audioSource,lineAttributes.lineSegments, true); //take 1 sample per vertex point
				Vector3 firstPos = Vector3.zero; // the first position we use
				for(int j = 0; j < lineAttributes.lineSegments-1; j++)
				{
					
					float rad = Mathf.Deg2Rad*angle;
					float x = Mathf.Cos(rad)*thisRadius;
					float y = samples[j]*maxHeight;
					float z = Mathf.Sin(rad)*thisRadius;
					Vector3 pos = this.transform.position + this.transform.right*x + this.transform.up*y + this.transform.forward*z;
					lines[i].SetPosition(j,pos);
					angle += angleStep;
					if(j == 0)
					{
						firstPos = pos;
					}
				}
				lines[i].SetPosition(lineAttributes.lineSegments-1,firstPos); // set the last pos = to the first pos.
			}
		}

		//display audio across all rings, damped by distance from the center
		void DampWave()
		{
			float radiusStep = radius / (numLines-1);
			float angleStep = 360f / lineAttributes.lineSegments;
			float[] samples = AudioSampler.instance.GetAudioSamples(audioSource,lineAttributes.lineSegments, true); //take 1 sample per vertex point

			for(int i = 0 ; i < numLines; i++)
			{
				float angle = 0; // 
				float thisRadius = radiusStep*i; // the radius of this ring
				Vector3 firstPos = Vector3.zero; // the first position we use
				float dampening = (1-(float)i/(numLines-1)); // goes from 1 to 0. dampens the height
				for(int j = 0; j < lineAttributes.lineSegments-1; j++)
				{
					
					float rad = Mathf.Deg2Rad*angle;
					float x = Mathf.Cos(rad)*thisRadius;
					float y = samples[j]*maxHeight*dampening;
					float z = Mathf.Sin(rad)*thisRadius;
					Vector3 pos = this.transform.position + this.transform.right*x + this.transform.up*y + this.transform.forward*z;
					lines[i].SetPosition(j,pos);
					angle += angleStep;
					if(j == 0)
					{
						firstPos = pos;
					}
				}
				lines[i].SetPosition(lineAttributes.lineSegments-1,firstPos); // set the last pos = to the first pos.
			}

		}

		//move rings up and down, based on audio
		void Bounce()
		{
			float radiusStep = radius / (numLines-1);
			float angleStep = 360f / (lineAttributes.lineSegments-1);
			for(int i = 0; i < numLines; i++)
			{
				float angle = 0; // 
				float thisRadius = radiusStep*i; // the radius of this ring
				float[] samples = AudioSampler.instance.GetAudioSamples(audioSource,numLines, true); //take 1 sample per vertex point
				float y = samples[i]*maxHeight;
				for(int j = 0; j < lineAttributes.lineSegments; j++)
				{
					float rad = Mathf.Deg2Rad*angle;
					float x = Mathf.Cos(rad)*thisRadius;
					float z = Mathf.Sin(rad)*thisRadius;
					Vector3 pos = this.transform.position + this.transform.right*x + this.transform.up*y + this.transform.forward*z;
					lines[i].SetPosition(j,pos);
					angle += angleStep;
				}
			}
		}



		void OnDrawGizmos()
		{
			if(gizmos)
			{
				Gizmos.color = Color.green;
				Gizmos.DrawSphere (this.transform.position, 1);
			}
		}

		//create "numlines" circles, from a radius of 0 to "radius".
		void CreatePad()
		{
			float radiusStep = radius / (numLines-1);
			float angleStep = 360f / (lineAttributes.lineSegments-1);
			for(int i = 0; i < numLines; i++)
			{
				float angle = 0; // 
				float thisRadius = radiusStep*i; // the radius of this ring
				for(int j = 0; j < lineAttributes.lineSegments; j++)
				{
					float rad = Mathf.Deg2Rad*angle;
					float x = Mathf.Cos(rad)*thisRadius;
					float y = Mathf.Sin(rad)*thisRadius;

					Vector3 pos = this.transform.position + this.transform.right*x + this.transform.forward*y;
					lines[i].SetPosition(j,pos);
					angle += angleStep;
				}
			}
		}

		//create a new lineRenderer, with the given color
		LineRenderer NewLine(Color c)
		{
			GameObject line = new GameObject ();
			line.transform.position = this.transform.position;
			line.transform.rotation = this.transform.rotation;
			line.hideFlags = HideFlags.HideInHierarchy;
			LineRenderer lr = line.AddComponent<LineRenderer> ();
			if(lineAttributes.lineMat == null)
			{
				lr.material = new Material(Shader.Find("Particles/Additive"));
			}
			else
			{
				lr.material = lineAttributes.lineMat;
			}
			lr.SetColors (c, c);
			lr.SetWidth (lineAttributes.startWidth,lineAttributes.endWidth);
			lr.SetVertexCount (lineAttributes.lineSegments);

			return lr;
		}
	}
}

