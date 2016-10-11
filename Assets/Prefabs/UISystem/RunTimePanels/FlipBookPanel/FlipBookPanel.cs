using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(PdfViewer))]
public class FlipBookPanel :MonoBehaviour
{
    public Button openBtn;
    public Button closeButton;
    public GameObject panel;
    public Book m_Book;

    private AutoFlip m_Flip;
    private PdfViewer pdfViewr;
    void Start()
    {
        closeButton.onClick.AddListener(delegate()
        {
            panel.gameObject.SetActive(false);
        });

        panel.gameObject.SetActive(false);
        m_Flip = m_Book.GetComponent<AutoFlip>();

        openBtn.onClick.AddListener(ShowBook);

        pdfViewr = GetComponent<PdfViewer>();

        InitBook();
    }
    private void InitBook()
    {
        m_Book.currentPage = 1;
        m_Book.bookPages = new Sprite[pdfViewr.GetBookPage()];
        m_Book.InitBook();
        m_Flip.InitFlip();
    }

    private void ShowBook()
    {
        pdfViewr.AddSpriteRequire(0, (x) => m_Book.bookPages[0] = x);
        pdfViewr.AddSpriteRequire(1, (x) => { m_Book.bookPages[1] = x;
            panel.gameObject.SetActive(true);
        });
    }

    public void OnChangePage(bool isNext)
    {
        if (isNext)
        {
            if (m_Book.bookPages.Length > m_Book.currentPage + 1)
                pdfViewr.AddSpriteRequire(m_Book.currentPage + 1, (x) => m_Book.bookPages[m_Book.currentPage + 1] = x);
            if (m_Book.bookPages.Length > m_Book.currentPage + 2)
                pdfViewr.AddSpriteRequire(m_Book.currentPage + 2, (x) => { m_Book.bookPages[m_Book.currentPage + 2] = x; m_Flip.FlipRightPage(); });
        }
        else
        {
            m_Flip.FlipLeftPage();
        }
    }
}
