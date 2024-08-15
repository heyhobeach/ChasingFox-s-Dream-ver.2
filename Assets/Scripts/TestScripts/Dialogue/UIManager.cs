using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Text.RegularExpressions;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using static System.Net.WebRequestMethods;
using static UnityEditor.Timeline.TimelinePlaybackControls;

public class UIManager : MonoBehaviour
{
    /// <summary>
    /// 선택지 생성한 오브젝트 담는 배열
    /// </summary>
    TMP_Text[] ContentArr = null;
    /// <summary>
    /// 다이얼로그 상자 크기
    /// </summary>
    float size;

    int select_count;
    //string test_str;
    /// <summary>
    /// 텍스트 오브젝트 이동 애니메이션 시간
    /// </summary>
    [SerializeField]
    private float duration = 3f;
    //public Text
    public TMP_Text namemesh;
    public TMP_Text content;

    public bool is_select_show = false;
    public bool is_closing = false;

    IEnumerator co = null;

    public IEnumerator co_closeAinm;

    Vector3 start_pos, end_pos;

    //public delegate void TestDel();
    //public TestDel testDel;

    private delegate void delayDelegeate();


    [SerializeField]
    public float typing_speed = 0.05f;
    //public InteractionEvent interactionEvent;

    // Start is called before the first frame update
    void Start()
    {
        //testDel = ActTest;
        co = Typing("");
        ContentArr = new TMP_Text[1];
        size= content.rectTransform.rect.size.y;
    }
    private void Awake()
    {
        is_closing = false;
        //co_closeAinm = ClosingAnim();
    }

    // Update is called once per frame

    public void Setname(string name)
    {
        namemesh.text = name;
    }

    public void SetContent(string _content)
    {
        StopCoroutine(co);
        co = Typing(_content);
        StartCoroutine(co);
    }
    public void SetContent(string[] _contentArr)//배열로 받을 예정
    {
        StopCoroutine(co);
        CreatSelect(_contentArr);
        co = TextSliding(_contentArr);
        StartCoroutine(co);
    }
    public void ChangeText(int countNum)//화살표 맞게 글자 색 변경하는 부분
    {
        TMP_Text TMP;
        content.color = Color.gray;
        for (int i = 0; i < select_count; i++)
        {
            TMP = content.transform.parent.GetChild(i).GetComponent<TMP_Text>();
            if (i == countNum)
            {
                TMP.color = Color.black;
            }
            else
            {
                TMP.color = Color.gray;
            }

        }
    }

    public void UpArrow(ref int countNum)
    {
        if (is_select_show) return;
        countNum--;
        ChangeText(countNum);

    }
    public void DownArrow(ref int countNum)
    {
        if (is_select_show) return;
        countNum++;
        ChangeText(countNum);
    }

    IEnumerator Typing(string str)
    {
        GameObject fixedVertical = content.transform.parent.gameObject;
        fixedVertical.GetComponent<VerticalLayoutGroup>().enabled = true;
        //첫 설정때 contentArr 설정 필요 지금 contentArr이 아무것도 없다고 되어있음 따라서 contentArr[0]에는 content가 들어가야함
        Debug.Log(str);
        Debug.Log(ContentArr.Length);
        if (ContentArr.Length>1)//사유 오브젝트 없음
        {
          DestroySelectBox();
        
        }
        content.text = null;
        if (content.color != Color.black)
        {
            content.color = Color.black;
        }
        if (str == "")
        {
            yield return null;
        }
        for (int i = 0; i < str.Length; i++)
        {
            content.text += str[i];
            yield return new WaitForSeconds(typing_speed);
        }
        Debug.Log("타이핑 종료");
    }
    void DestroySelectBox()
    {
        //Debug.Log("선택 위치"+c)
        Debug.Log("파괴");
        for(int i = 1; i < ContentArr.Length; i++)
        {
            Destroy(ContentArr[i].transform.gameObject);
        }
        ContentArr = new TMP_Text[1];
        //ContentArr = null;
    }
    public void CloseSelceet(int choseIndex)
    {
        if (ContentArr.Length == 1) return;
        Debug.Log("선택번호"+choseIndex);
        //int childs = this.gameObject.transform.parent.transform.childCount;
        int childs = content.transform.parent.transform.childCount;
        Debug.Log("자식수"+childs);
        is_closing = true;
        Debug.Log("선택 자식"+content.transform.parent.GetChild(choseIndex).GetComponent<TMP_Text>().text);
        GameObject selectobj = content.transform.parent.GetChild(choseIndex).gameObject;
        string tmpstr = selectobj.GetComponent<TMP_Text>().text;
        content.text = tmpstr;
        start_pos=selectobj.transform.position;
        end_pos =content.transform.parent.GetChild(childs-1).transform.position;
        Debug.Log(string.Format("start_pos {0} end_pos{1}",start_pos,end_pos));
        DestroySelectBox();
        //StartCoroutine(ClosingAnim(()=>{}));
        //StartCoroutine(ClosingAnim(testDel));
        //0 1 2 아래로 -50x
        //선택한 번호의 위치 계산 
        //해당 위치로 스무스하게 이동
        //enumerator를 이용해 보간 이동을 아래로 하도록 위치는 텍스트 3번째 기본 텍스트 위치 기준
    }
    public IEnumerator ClosingAnim(Action Act=null)
    //IEnumerator ClosingAnim()
    {
        is_closing = true;
        //yield return new WaitForSecondsRealtime(1);
        GameObject fixedVertical = content.transform.parent.gameObject;
        fixedVertical.GetComponent<VerticalLayoutGroup>().enabled = false;
        float t = 0;
        float _duration = 1;
        content.transform.position = start_pos;
        Debug.Log("출발 위치 " + start_pos+"content 위치"+content.transform.position);
        while (t < _duration)
        {
            //보간이동 내용
            t = t / _duration;
            //1 - (1 - x) * (1 - x);
            float lerp_y=Mathf.Lerp(content.transform.position.y, end_pos.y, t);
            Debug.Log("lerp y is" + lerp_y);
            content.transform.position = new Vector3(content.transform.position.x, lerp_y, content.transform.position.z);
            t += Time.deltaTime;
            yield return null;
        }
        Debug.Log("1초 끝");
        is_closing = false;
        if (Act == null)
        {
            yield return null;
        }
        else
        {
            Act();
        }
    }

    public void ActTest()
    {
        Debug.Log("액션 테스트");
    }
    void CreatSelect(string[] strArr)
    {
        select_count = strArr.Length;
        Debug.Log("strArr length " + strArr.Length);    
        //Debug.Log("strArr[0]" + strArr[0]);
        //for(int i=0;i<strArr.Length;i++)
        content.text = strArr[0];
        content.color = Color.black;
        ContentArr = new TMP_Text[strArr.Length];
        ContentArr[0] = content;
        for (int i = 1; i < select_count; i++)//오브젝트 생성과 텍스트 배치
        {
            Debug.Log(content.transform.parent.name);
            TMP_Text select = Instantiate(content, this.transform.position-new Vector3(this.transform.position.x-100,this.transform.position.y,this.transform.position.z), Quaternion.identity);
            ContentArr[i] = select;
            select.transform.SetParent(content.transform.parent);
            //      select.transform.parent = content.transform.parent;
            select.text = strArr[i];
            select.color = Color.gray;
        }
    }

    IEnumerator TextSliding(string[] strArr)//배열로 받을 예정
    {
        //is_select_show = true;
        float delta = 0;
        GameObject fixedVertical = content.transform.parent.gameObject;
        fixedVertical.GetComponent<VerticalLayoutGroup>().enabled = false;
        Debug.Log("여기" + ContentArr[0]);
        Debug.Log("여기" + ContentArr[1]);

        Debug.Log("여기" + ContentArr[2]);
        float endPos = ContentArr[0].transform.position.x;
        int count = 0;
        Debug.Log("size" + size);
        while (delta <= duration&(count<3))
        {
            float t = delta / duration;
            t = 1 - Mathf.Pow(1 - t, 3);
            float current = Mathf.Lerp(this.transform.position.x - 100, endPos, t);//시작위치,도착위치,t
            ContentArr[count].transform.position = new Vector3(current, ContentArr[0].transform.position.y-(size*count), ContentArr[count].transform.position.z);
            delta += Time.deltaTime;
            if(delta> duration)
            {
                Debug.Log("증가");
                count++;
                delta = 0;
                continue;
            }
            //Debug.Log(ContentArr[0].text + "위치 " + current);
            yield return null;
        }
        fixedVertical.GetComponent<VerticalLayoutGroup>().enabled = true;
        is_select_show = false;


    }
}