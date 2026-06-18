using UnityEngine;
using System.Collections;
// ⭐️【新設】「リスト（List）」という、カードの束を管理するパックを使えるようにします！
using System.Collections.Generic;

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

    // ⭐️【新設】ゲームが始まった瞬間に動く部屋
    void Start()
    {
        ShuffleAndAssignCards();
    }

    // ⭐️【新設】カードを集めて、シャッフルして、数字を配る魔法のメソッド
    private void ShuffleAndAssignCards()
    {
        // ① 画面にある「Card」スクリプトがついたオブジェクトを全部見つけて、リスト（束）にする！
        List<Card> allCards = new List<Card>(Object.FindObjectsByType<Card>(FindObjectsSortMode.None));

        // ⭐️【新設】② 16枚分の数字を入れるための「数字専用のメモ用紙（リスト）」を用意する
        List<int> numbers = new List<int>();

        // ⭐️【新設】③ 1から8までの数字を、2回ずつループしてメモに書き込む！
        // （これでメモ帳の中身が [1,1,2,2,3,3,4,4,5,5,6,6,7,7,8,8] になります）
        for (int i = 1; i <= 8; i++)
        {
            numbers.Add(i); // 1回目
            numbers.Add(i); // 2回目
        }

        // ⭐️【新設】④【超重要】数字のメモ帳をバラバラにシャッフルする！
        // 「フィッシャー・イェーツのシャッフル」という有名なアルゴリズム（手順）です
        for (int i = numbers.Count - 1; i > 0; i--)
        {
            // 0からi番目の中から、ランダムで1つ選ぶ
            int randomIndex = Random.Range(0, i + 1);

            // 選ばれた数字と、今の位置の数字を「入れ替える」！
            int temp = numbers[i];
            numbers[i] = numbers[randomIndex];
            numbers[randomIndex] = temp;
        }

        // ⭐️【新設】⑤ バラバラになった数字を、16枚のカードに上から順番に配る！
        for (int i = 0; i < allCards.Count; i++)
        {
            // カード側の cardNumber を、シャッフル済みの数字で上書きしちゃう！
            allCards[i].cardNumber = numbers[i];
        }

        Debug.Log("16枚のカードに、シャッフルした数字を配り終わったよ！");
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