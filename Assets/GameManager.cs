using UnityEngine;

public class GameManager : MonoBehaviour
{
    // いま何枚目にめくられたカードかを覚えておく部屋（1枚目？2枚目？）
    private int flippedCount = 0;

    // 【仕掛け】カードがクリックされたら、Cardスクリプトから呼び出される命令
    public void CardFlipped(Card clickedCard)
    {
        // めくられた枚数を1増やす
        flippedCount++;
        Debug.Log("レフェリー「いま " + flippedCount + " 枚目のカードがめくられたよ！」");

        if (flippedCount == 1)
        {
            // 1枚目のときの処理（とりあえずここまで！）
            Debug.Log("レフェリー「1枚目の数字を覚えたよ」");
        }
        else if (flippedCount == 2)
        {
            // 2枚目のときの処理
            Debug.Log("レフェリー「2枚目がめくられたから、判定するよ！」");

            // 枚数をリセットして0に戻す（次のペアのために）
            flippedCount = 0;
        }
    }
}