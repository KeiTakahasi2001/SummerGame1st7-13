using UnityEngine;
using System.Collections;
// ⭐️【新設】「リスト（List）」という、カードの束を管理するパックを使えるようにします！
using System.Collections.Generic;
using TMPro; // ⭐️【新設】TextMeshProを使うための魔法の呪文！

public class GameManager : MonoBehaviour
{
    private int flippedCount = 0;
    private Card firstCard;
    //1秒待っている間、入力を禁止するための「鍵」
    // 最初はロックしていない（false）状態
    private bool isLocking = false;

    // 手数とタイマーのための変数たち
    private int flipCount = 0;       // めくった回数（手数）
    private float timer = 0f;        // 経過時間（タイマー）
    private bool isGameClear = false; // ゲームがクリアされたかどうかの旗

    // Unityの画面で作ったテキスト部品をここにドラッグ＆ドロップで紐付けます
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI timerText;

    // 外部のカードから「いま画面はロックされてる？」と確認するための窓口
    public bool IsLocking
    {
        get { return isLocking; }
    }

    // ⭐️【新設】ゲームが始まった瞬間に動く部屋
    void Start()
    {
        ShuffleAndAssignCards();
        UpdateScoreText();
    }

    void Update()
    {
        // もしゲームクリア「じゃない」なら、時間を進める！
        if (isGameClear == false)
        {
            timer += Time.deltaTime; // ⏰ 前回の居残りでやった、時間を足していく魔法！
            timerText.text = "タイム: " + timer.ToString("F1") + "秒"; // 「F1」は小数第1位まで表示するお守り
        }
    }

    // ⭐️【新設】カードを集めて、シャッフルして、数字を配る魔法のメソッド
    private void ShuffleAndAssignCards()
    {
        // ① 画面にあるカードを全員集合させる（16枚だったり、20枚だったりする）
        List<Card> allCards = new List<Card>(Object.FindObjectsByType<Card>(FindObjectsSortMode.None));

        // ⭐️【新設】集まったカードの枚数から、必要なペアの数を自動計算する！
        // （16枚なら 16 / 2 = 8ペア、20枚なら 20 / 2 = 10ペア になる！）
        int pairCount = allCards.Count / 2;

        List<int> numbers = new List<int>();

        // ⭐️【ここを修正！】「8」と直接書くのをやめて、自動計算した「pairCount」にする！
        for (int i = 1; i <= pairCount; i++)
        {
            numbers.Add(i);
            numbers.Add(i);
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

            // ⭐️【これがない！】2枚めくったので手数を1増やして、画面を書き換える！
            flipCount++;
            UpdateScoreText();

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
        // ① 1秒間待たずに、すぐに「判定中」という雰囲気を出すために0.3秒だけ待つ
        yield return new WaitForSeconds(0.3f);

        // ⭐️【新設】ここ！カードを実際に消す前に、2枚に「弾けろ！」という命令を送る！
        card1.PopUpAnimation();
        card2.PopUpAnimation();

        // ② アニメーションが動いている間、ちょっとだけ待つ（0.2秒待つ）
        yield return new WaitForSeconds(0.2f);

        // ③ ここでカードを画面から消し去る
        card1.DeleteCard();
        card2.DeleteCard();

        // ④ カードが消えたあと、もし画面に1枚もカードが残ってないならゲームクリア！
        if (Object.FindObjectsByType<Card>(FindObjectsSortMode.None).Length == 0)
        {
            isGameClear = true; // ⏰ タイマーを止める！
            Debug.Log("ゲームクリア！おめでとう！");
        }

        // ③ 消し終わったら、ロックを解除して次の入力を受け付ける
        isLocking = false;

        Debug.Log("「ペアを消去したから、ロックを解除したよ！」");
    }
    private void UpdateScoreText()
    {
        scoreText.text = "手数: " + flipCount + "回";
    }
}