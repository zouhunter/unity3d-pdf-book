using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;


using System.Drawing;
using System.Drawing.Imaging;
using O2S.Components.PDFRender4NET;
using System.Collections.Generic;
using System.IO;

public class PdfController : IPdfController
{
    private string pdfFileName;
    private PictureConfig picureConfig;
    private PDFFile pdfFile;
    private string tempPath;
    public PdfController(string pdfFileName, PictureConfig config)
    {
        this.pdfFileName = pdfFileName;
        this.picureConfig = config;
        tempPath = Application.streamingAssetsPath + "/ImageCache/" + Path.GetFileNameWithoutExtension(this.pdfFileName);
        Directory.CreateDirectory(tempPath);
        pdfFile = PDFFile.Open(this.pdfFileName);
    }

    public int Page
    {
        get
        {
            return pdfFile != null ? pdfFile.PageCount : 0;
        }
    }
    public bool GetFilePath(int page, UnityAction<byte[]> fileAction)
    {
        if (pdfFile == null) return false;

        Debug.Log(1);
        Bitmap map;
        map = pdfFile.GetPageImage(page, picureConfig.definition);
        Debug.Log(2);
        if (map == null) return false;
        Debug.Log(3);

        map = SetPictureAlpha(map, (int)picureConfig.transparent);
        Debug.Log(4);

        byte[] bytes = BitMapToArray(map);
        Debug.Log(5);

        if (fileAction != null) fileAction(bytes);
        return true;
    }
    public void ClearOldFiles()
    {
        Directory.Delete(tempPath);
    }

    private byte[] BitMapToArray(Bitmap bmp)
    {
        MemoryStream ms = new MemoryStream();
        byte[] bytes;
        bmp.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
        bytes = ms.GetBuffer();  //byte[]   bytes=   ms.ToArray(); 这两句都可以，至于区别么，下面有解释
        ms.Close();
        return bytes;
    }
    /// <summary>  
    /// 设置图片的透明度  
    /// </summary>  
    /// <param name="image">原图</param>  
    /// <param name="alpha">透明度0-255</param>  
    /// <returns></returns>  
    private Bitmap SetPictureAlpha(Bitmap image, int alpha)
    {
        //颜色矩阵  
        float[][] matrixItems =
        {
                   new float[]{1,0,0,0,0},
                   new float[]{0,1,0,0,0},
                   new float[]{0,0,1,0,0},
                   new float[]{0,0,0,alpha/255f,0},
                   new float[]{0,0,0,0,1}
               };
        ColorMatrix colorMatrix = new ColorMatrix(matrixItems);
        ImageAttributes imageAtt = new ImageAttributes();
        imageAtt.SetColorMatrix(colorMatrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);
        Bitmap bmp = new Bitmap(image.Width, image.Height);
        System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(bmp);
        g.DrawImage(image, new Rectangle(0, 0, image.Width, image.Height),
                0, 0, image.Width, image.Height, GraphicsUnit.Pixel, imageAtt);
        g.Save();
        g.Dispose();
        return bmp;
    }
}

