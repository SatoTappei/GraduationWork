using Game;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommentDisplayerテスト : MonoBehaviour
{
    CommentSpreadSheetLoader _commentLoader;
    CommentPool _commentPool;
    CommentPlacer _commentPlacer;

    // ゲーム開始時、スプレッドシートから非同期でコメントを読み込んでいる。
    public bool IsReady => !_commentLoader.IsLoading;

    void Awake()
    {
        _commentLoader = GetComponent<CommentSpreadSheetLoader>();
        _commentPool = GetComponent<CommentPool>();
        _commentPlacer = new CommentPlacer();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            for (int i = 0; i < 100; i++)
            {
                if (_commentPool.TryPop(out DisplayedComment displayedComment))
                {
                    displayedComment.SetText("あ、やめなよ。");
                    _commentPlacer.Set(displayedComment);
                }
            }

            _commentPlacer.Play();
        }
    }
}
