using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

public class PdfViewer : MonoBehaviour
{
    public PictureConfig pictureConfig;
    private IPdfController controller;
    private Queue<KeyValuePair<int, string>> waitLoadTexture = new Queue<KeyValuePair<int,string>>();
    private Dictionary<int, Sprite> spriteDic = new Dictionary<int, Sprite>();
    private Dictionary<int, UnityAction<Sprite>> waitHandEvent = new Dictionary<int, UnityAction<Sprite>>();
    private Queue<int> threadDown = new Queue<int>();
    private Thread handlePdfThread;
    void Start()
    {
        controller = new PdfController(Application.streamingAssetsPath + "/PdfTest/Csharp网络编程.pdf", pictureConfig);
        handlePdfThread = new Thread(ThreadLoop);
        handlePdfThread.Start();
        //StartCoroutine(EnumeratorLoop());
    }

    /// <summary>
    /// 打开书本
    /// </summary>
    /// <param name="path"></param>
    public void OpenBook(string path)
    {
        controller = new PdfController(Application.streamingAssetsPath + "/PdfTest/Csharp网络编程.pdf", pictureConfig);
    }
    IEnumerator EnumeratorLoop()
    {
        while (true)
        {
            if (threadDown.Count > 0)
            {
                int handle = threadDown.Dequeue();
                controller.GetFilePath(handle, (x) => waitLoadTexture.Enqueue(new KeyValuePair<int, string>(handle, x)));
            }
            yield return new WaitForSeconds(1);
        }
    }
    private void ThreadLoop()
    {
        while(true)
        {
            if (threadDown.Count > 0)
            {
                int handle = threadDown.Dequeue();
                controller.GetFilePath(handle, (x) => waitLoadTexture.Enqueue(new KeyValuePair<int, string>(handle, x)));
            }
            Thread.Sleep(1);
        }
    }
    /// <summary>
    /// 添加文件请求
    /// </summary>
    /// <param name="page"></param>
    /// <param name="Func"></param>
    public void AddSpriteRequire(int page,UnityAction<Sprite> Func)
    {
        if (waitHandEvent.ContainsKey(page)) return;
        if (spriteDic.ContainsKey(page) && Func != null)
        {
            Func(spriteDic[page]);
        }
        else
        {
            waitHandEvent.Add(page, Func);
            threadDown.Enqueue(page);
        }
    }
    public int GetBookPage()
    {
        return controller.Page;
    }

    /// <summary>
    /// 下载图片
    /// </summary>
    /// <param name="readIndex"></param>
    /// <returns></returns>
    IEnumerator DownLoadTexture(string path, UnityAction<Sprite> getTexture)
    {
        string TexturePath = "file:///" + path;
        WWW www = new WWW(TexturePath);
        yield return www;
        if (www.error == null && www.texture != null)
        {
            Texture2D texture = www.texture;
            Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.one * 0.5f);
            getTexture(sprite);
            
        }
        else
        {
            Debug.Log(www.error);
        }
    }

  
    void Update()
    {
        if (waitLoadTexture.Count > 0)
        {
            KeyValuePair<int, string> action = waitLoadTexture.Dequeue();
            UnityAction<Sprite> Func;
            if (waitHandEvent.TryGetValue(action.Key,out Func))
            {
                Func += (x) => { spriteDic.Add(action.Key, x); };
                StartCoroutine(DownLoadTexture(action.Value, Func));
                waitHandEvent.Remove(action.Key);
            }
        }
    }
}
