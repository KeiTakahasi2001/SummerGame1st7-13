using UnityEngine;
using System.Collections;
// ⭐️【新設】「リスト（List）」という、カードの束を管理するパックを使えるようにします！
using System.Collections.Generic;
using TMPro; // ⭐️【新設】TextMeshProを使うための魔法の呪文！
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private int flippedCount = 0;
    private Card firstCard;
    //1秒待っている間、入力を禁止するための「鍵」
    // 最初はロックしていない（false）状態
    private bool isLocking = false;

    // 手数とタイマーのための変数たち（staticにしてクリア画面に持っていけるようにする！）
    public static int flipCount = 0;// めくった回数（手数）
    public static float timer = 0f;// 経過時間（タイマー）
    private bool isGameClear = false; // ゲームがクリアされたかどうかの旗
    private float startDelayTimer = 0f; // 最初の「ちょっと待つ時間」を数えるタイマー
    private bool isReady = true;        // いま準備中（待機中）かどうかの旗

    // Unityの画面で作ったテキスト部品をここにドラッグ＆ドロップで紐付けます
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI readyText;

    // 外部のカードから「いま画面はロックされてる？」と確認するための窓口
    public bool IsLocking
    {
        get { return isLocking; }
    }

    // ⭐️【新設】ゲームが始まった瞬間に動く部屋
    void Start()
    {
        flipCount = 0;
        timer = 0f;// 新しくゲームを始めるたびに、記録を0にリセットする！

        ShuffleAndAssignCards();
        UpdateScoreText();
    }

    void Update()
    {
        if (isReady == true) // ⭐️【カウントダウン中（最初の3秒間）】
        {
            startDelayTimer += Time.deltaTime;
            int displayCount = (int)(3.0f - startDelayTimer + 1.0f);
            readyText.text = displayCount.ToString();// ❶ 真ん中のデカ文字に「3、2、1」を表示（ひらがなで文字化け対策！）

            timerText.text = "タイム: " + timer.ToString("F1") + "秒";// ❷ 左上のタイマーには「タイム: 0.0秒」って静かに表示しておく！

            if (startDelayTimer >= 3.0f)
            {
                isReady = false; // 本編スタート！
                readyText.gameObject.SetActive(false); // ⭐️【新設】用が済んだので真ん中のデカ文字を消す！
            }
            return;
        }

        // ❶ もしすでにゲームクリアしているなら、これ以上下の処理は何もせずにここで終わり！
        if (isGameClear == true) return;

        // ❷ ⏰ タイムを毎フレーム進めて、画面に表示する！
        timer += Time.deltaTime;
        timerText.text = "タイム: " + timer.ToString("F1") + "秒";

        // ❸ ⭐️【新設】もしタイマーが「60秒」を超えたら…恐怖のタイムアップ！
        if (timer >= 60.0f)
        {
            isGameClear = true; // タイマーを止めるためにフラグをONにする
            Debug.Log("タイムアップ！ゲームオーバー！");

            // ゲームオーバー画面へ強制ジャンプ！
            UnityEngine.SceneManagement.SceneManager.LoadScene("GameOverScene");
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

        //「8」と直接書くのをやめて、自動計算した「pairCount」にする！
        for (int i = 1; i <= pairCount; i++)
        {
            numbers.Add(i);
            numbers.Add(i);
        }

        // ④【超重要】数字のメモ帳をバラバラにシャッフルする！
        // 「フィッシャー・イェーツのシャッフル」という有名なアルゴリズム（手順）です
        for (int i = numbers.Count - 1; i > 0; i--)
        {
            int randomIndex = Random.Range(0, i + 1);// 0からi番目の中から、ランダムで1つ選ぶ

            // 選ばれた数字と、今の位置の数字を「入れ替える」！
            int temp = numbers[i];
            numbers[i] = numbers[randomIndex];
            numbers[randomIndex] = temp;
        }

        for (int i = 0; i < allCards.Count; i++)// ⑤ バラバラになった数字を、16枚のカードに上から順番に配る！
        {
            allCards[i].cardNumber = numbers[i];// カード側の cardNumber を、シャッフル済みの数字で上書きしちゃう！
        }

        Debug.Log("16枚のカードに、シャッフルした数字を配り終わったよ！");
    }

    public void CardFlipped(Card clickedCard)
    {
        // ⭐️【追加】まだ準備中（3秒経ってない）なら、カードをめくる処理を無視してここで追い返す！
        if (isReady == true) return;

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

            // 2枚めくったので手数を1増やして、画面を書き換える！
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

        int activeCards = 0;// ④今画面に残っている「ボタンがまだオン（有効）なカード」をその場で数える！
        foreach (Card c in Object.FindObjectsByType<Card>(FindObjectsSortMode.None))
        {
            if (c.GetComponent<UnityEngine.UI.Button>().enabled == true)
            {
                activeCards++;
            }
        }

        // ⭐️ 有効なカードが0枚（全部めくり終わった）ならクリア！
        if (activeCards == 0)
        {
            isGameClear = true; //  タイマーを止める！
            Debug.Log("ゲームクリア！おめでとう！");

            yield return new WaitForSeconds(1.0f);// 画面を切り替える前に、2.0秒だけ「余韻の時間」を待つ！

            UnityEngine.SceneManagement.SceneManager.LoadScene("ClearScene"); // 自動でジャンプ！
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