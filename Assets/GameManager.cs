using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour
{
    private int flippedCount = 0;
    private Card firstCard;

    // ⭐️【新設】1秒待っている間、入力を禁止するための「鍵」
    // 最初はロックしていない（false）状態
    private bool isLocking = false;

    // ⭐️【新設】外部のカードから「いま画面はロックされてる？」と確認するための窓口
    public bool IsLocking
    {
        get { return isLocking; }
    }

    public void CardFlipped(Card clickedCard)
    {
        flippedCount++;
        Debug.Log("「いま " + flippedCount + " 枚目のカードがめくられたよ！」");

        if (flippedCount == 1)
        {
            firstCard = clickedCard;
            Debug.Log("「1枚目のカードを覚えたよ」");
        }
        else if (flippedCount == 2)
        {
            Debug.Log("「2枚目がめくられたから判定するよ！」");

            if (firstCard.cardNumber == clickedCard.cardNumber)
            {
                // ⭕️ 正解の時
                Debug.Log("「正解！！！ロックをかけて1秒後に消去するよ」");

                // ⭐️ハズレの時と同じように、1秒待つ間に連打されないようにロックをかける！
                isLocking = true;

                // ⭐️【新設】正解の時専用の、1秒待って消すコルーチンをスタート！
                StartCoroutine(DeleteCardsAfterDelay(firstCard, clickedCard));

                flippedCount = 0;
                firstCard = null;
            }
            else
            {
                // ❌ ハズレの時
                Debug.Log("「ハズレ！ロックをかけて1秒待つよ」");

                // ⭐️【超重要】ハズレが確定した瞬間、ロックの鍵を「true（ロック中）」にする！
                isLocking = true;

                StartCoroutine(HideCardsAfterDelay(firstCard, clickedCard));

                flippedCount = 0;
                firstCard = null;
            }
        }
    }

    private IEnumerator HideCardsAfterDelay(Card card1, Card card2)
    {
        yield return new WaitForSeconds(1.0f);

        card1.HideCard();
        card2.HideCard();

        // ⭐️【超重要】1秒経って裏に戻し終わったら、ロックを「false（解除）」にする！
        isLocking = false;

        Debug.Log("「裏に戻したから、ロックを解除したよ！」");
    }

    // ⭐️【新設】1秒待ってから、2枚のカードを画面から消し去る魔法
    private IEnumerator DeleteCardsAfterDelay(Card card1, Card card2)
    {
        // ① 1秒間、じっと待つ
        yield return new WaitForSeconds(1.0f);

        // ② さっきカード側に作った「消えろ！」という命令（メソッド）を呼び出す！
        card1.DeleteCard();
        card2.DeleteCard();

        // ③ 消し終わったら、ロックを解除して次の入力を受け付ける
        isLocking = false;

        Debug.Log("「ペアを消去したから、ロックを解除したよ！」");
    }
}