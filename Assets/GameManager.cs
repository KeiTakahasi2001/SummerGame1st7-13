using UnityEngine;
using System.Collections; // 👈 ⭐️「時間を操る機能（コルーチン）」を使うためにこれが必要です！

public class GameManager : MonoBehaviour
{
    private int flippedCount = 0;
    private Card firstCard;

    public void CardFlipped(Card clickedCard)
    {
        flippedCount++;
        Debug.Log("「いま " + flippedCount + " 枚目のカードがめくられたよ！」");

        if (flippedCount == 1)
        {
            firstCard = clickedCard;
            Debug.Log("「1枚目のカード（数字: " + firstCard.cardNumber + "）を覚えたよ」");
        }
        else if (flippedCount == 2)
        {
            Debug.Log("「2枚目がめくられたから、1枚目と判定するよ！」");

            if (firstCard.cardNumber == clickedCard.cardNumber)
            {
                // ⭕️ 正解の時はそのまま（画面から消すなどは後日！）
                Debug.Log("「正解！！！ペア成立です！」");

                // 次のペアのためにリセット
                flippedCount = 0;
                firstCard = null;
            }
            else
            {
                // ❌ ハズレの時
                Debug.Log("「ハズレ！1秒後に裏に戻す魔法（コルーチン）を呼び出すよ」");

                // ⭐️【新設】時間を操る魔法「コルーチン」をスタートする命令！
                StartCoroutine(HideCardsAfterDelay(firstCard, clickedCard));

                // ⭐️【重要】ハズレの時は、裏に戻るのを待っている間に「3枚目」を押されないように、
                // 判定中のカード以外のカウントをここで先にリセットしておきます
                flippedCount = 0;
                firstCard = null;
            }
        }
    }

    // ⭐️【新設】1秒待ってから、2枚のカードを裏に戻す「時間を操る魔法」
    private IEnumerator HideCardsAfterDelay(Card card1, Card card2)
    {
        // ① まずここで「1秒間」ゲームの時間をストップして待つ！
        yield return new WaitForSeconds(1.0f);

        // ② 1秒経ったら、それぞれのカードの「裏に戻れ！」という命令（メソッド）を呼び出す
        card1.HideCard();
        card2.HideCard();

        Debug.Log("「1秒経ったから、2枚とも裏に戻したよ！」");
    }
}