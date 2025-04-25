using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using VRC.Udon.Wrapper.Modules;

public class PlatformAutoLooper : MonoBehaviour
{
    [SerializeField] List<GameObject> m_prefabsToShowcase;

    [SerializeField] private int m_prefabIdx = 0;

    [SerializeField] private Vector3 m_bottomPosition;
    [SerializeField] private Vector3 m_topPosition;

    [SerializeField] private bool m_rising = false;

    [SerializeField] private bool m_isMoving = false;

    [SerializeField] private int m_framesToWaitAtPeak = 60;
    private int m_framesWaitedSoFar = 0;

    [SerializeField] private int m_framesToMove = 120;
    private int m_framesMovedSoFar = 0;

    // Start is called before the first frame update
    void Start()
    {
        m_isMoving = true;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        if (m_isMoving)
        {
            if (m_rising)
            {
                if (LerpPositionUntilDest(m_bottomPosition, m_topPosition))
                {
                    m_framesWaitedSoFar++;
                    if (m_framesWaitedSoFar >= m_framesToWaitAtPeak)
                    {
                        m_framesWaitedSoFar = 0;
                        m_rising = false;
                        m_framesMovedSoFar = 0;
                    }
                }
            }
            else
            {
                if (LerpPositionUntilDest(m_topPosition, m_bottomPosition))
                {
                    if (m_prefabsToShowcase.Count > 1)
                    {
                        Debug.Log("This is entered");
                        m_prefabsToShowcase[m_prefabIdx].SetActive(false);
                        //Wrapping increment
                        m_prefabIdx = (m_prefabIdx+1) % (m_prefabsToShowcase.Count);
                        m_prefabsToShowcase[m_prefabIdx].SetActive(true);

                        m_rising = true;
                        m_framesMovedSoFar = 0;
                    }
                }
            }
        }
    }

    public void TogglePlatformLooper()
    {
        m_isMoving = !m_isMoving;
    }

    //Returns true when at Dest.
    private bool LerpPositionUntilDest(Vector3 i_start, Vector3 i_dest)
    {
        gameObject.transform.localPosition = Vector3.Lerp(i_start, i_dest, ((float)m_framesMovedSoFar / (float)m_framesToMove));

        //Debug.Log("Moved so far: " + m_framesMovedSoFar);
        //Debug.Log("Division Result: " + m_framesToMove);

        if (m_framesMovedSoFar >= m_framesToMove)
        {
            return true;
        }

        m_framesMovedSoFar++;
        return false;
    }
}
