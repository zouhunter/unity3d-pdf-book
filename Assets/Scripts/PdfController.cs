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
        tempPath = Application.temporaryCachePath + "/ImageCache/" + Path.GetFileNameWithoutExtension(this.pdfFileName);
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
    public bool GetFilePath(int page, UnityAction<string> fileAction)
    {
        string imagePath = tempPath + "/" + page + "." + "png";
        if (File.Exists(imagePath))
        {
            if (fileAction != null) fileAction(imagePath);
            return true;
        }

        if (pdfFile == null) return false;

        Debug.Log(1);
        Bitmap map;
        map = pdfFile.GetPageImage(page, picureConfig.definition);
        Debug.Log(2);
        if (map == null) return false;
        Debug.Log(3);

        ImageFormat format = picureConfig.imageFormat ?? ImageFormat.Png;
        map = SetPictureAlpha(map, (int)picureConfig.transparent);
        Debug.Log(4);

        Debug.Log(imagePath);
        FileStream stream = new FileStream(imagePath,FileMode.OpenOrCreate);
        map.Save(stream, format);
        stream.Dispose();
        stream.Close();
        Debug.Log(5);

        if (fileAction != null) fileAction(imagePath);
        return true;
    }
    public void ClearOldFiles()
    {
        Directory.Delete(tempPath);
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
