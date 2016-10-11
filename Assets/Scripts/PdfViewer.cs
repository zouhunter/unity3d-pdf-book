using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

#if UNITY_STANDALONE
using System.Threading;
#endif

public class PdfViewer : MonoBehaviour
{
    public PictureConfig pictureConfig;
    private IPdfController controller;
    private Queue<KeyValuePair<int, byte[]>> waitLoadTexture = new Queue<KeyValuePair<int, byte[]>>();
    private Dictionary<int, Sprite> spriteDic = new Dictionary<int, Sprite>();
    private Dictionary<int, UnityAction<Sprite>> waitHandEvent = new Dictionary<int, UnityAction<Sprite>>();
    private Queue<int> threadDown = new Queue<int>();
#if UNITY_STANDALONE
    private Thread handlePdfThread;
#endif
    void Awake()
    {
        controller = new PdfController(Application.streamingAssetsPath + "/PdfTest/Csharp网络编程.pdf", pictureConfig);

#if UNTIY_STANDALONE
        handlePdfThread = new Thread(ThreadLoop);
        handlePdfThread.Start();
#else
        StartCoroutine(EnumeratorLoop());
#endif
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
                controller.GetFilePath(handle, (x) => waitLoadTexture.Enqueue(new KeyValuePair<int, byte[]>(handle, x)));
            }
            yield return new WaitForSeconds(1);
        }
    }
#if UNTIY_STANDALONE
    private void ThreadLoop()
    {
        while (true)
        {
            if (threadDown.Count > 0)
            {
                int handle = threadDown.Dequeue();
                controller.GetFilePath(handle, (x) => waitLoadTexture.Enqueue(new KeyValuePair<int, string>(handle, x)));
            }
            Thread.Sleep(1);
        }
    }
#endif
    /// <summary>
    /// 添加文件请求
    /// </summary>
    /// <param name="page"></param>
    /// <param name="Func"></param>
    public void AddSpriteRequire(int page, UnityAction<Sprite> Func)
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
    IEnumerator DownLoadTexture(byte[] path, UnityAction<Sprite> getTexture)
    {
        //string TexturePath = "file:///" + path;
        Texture2D texture = new Texture2D(200, 300);
        if(texture.LoadImage(path,false))
        {
            texture.Apply();
            Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.one * 0.5f);
            yield return null;
            getTexture(sprite);
        }
        //WWW www = new WWW(TexturePath);
        //yield return www;
        //if (www.error == null && www.texture != null)
        //{
        //    Texture2D texture = www.texture;
        //    Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.one * 0.5f);
        //    getTexture(sprite);

        //}
        //else
        //{
        //    Debug.Log(www.error);
        //}
    }


    void Update()
    {
        if (waitLoadTexture.Count > 0)
        {
            KeyValuePair<int, byte[]> action = waitLoadTexture.Dequeue();
            UnityAction<Sprite> Func;
            if (waitHandEvent.TryGetValue(action.Key, out Func))
            {
                Func += (x) => { spriteDic.Add(action.Key, x); };
                StartCoroutine(DownLoadTexture(action.Value, Func));
                waitHandEvent.Remove(action.Key);
            }
        }
    }
}

