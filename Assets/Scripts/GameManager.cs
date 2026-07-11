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

    // 💡【追加】さっき鳴らした数字を覚えておくためのメモ帳
    private int lastDisplayedCount = -1;

    // Unityの画面で作ったテキスト部品をここにドラッグ＆ドロップで紐付けます
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI readyText;

    // GameManagerにスピーカー（AudioSource）を持たせる
    private AudioSource audioSource;

    // お気に入りの効果音データを入れておく箱たち
    [Header("効果音シリーズ")]
    [SerializeField] private AudioClip correctSound;    // 正解（ティロリーン！）
    [SerializeField] private AudioClip incorrectSound;  // 不正解（ブブー）
    //[SerializeField] private AudioClip clearSound;      // クリア音
    //[SerializeField] private AudioClip gameOverSound;   // ゲームオーバー音

    // 💡【追加】カウントダウン用の音をセットする箱
    [SerializeField] private AudioClip countdownSound;  // 「3・2・1」の音（ピピッなど）
    [SerializeField] private AudioClip startVoiceSound;  // 「Go!」の音（パーン！など）

    // 外部のカードから「いま画面はロックされてる？」と確認するための窓口
    public bool IsLocking
    {
        get { return isLocking; }
    }

    // カードから「いま何枚目がめくられた？」を確認するための窓口
    public int FlippedCount
    {
        get { return flippedCount; }
    }

    // ゲームが始まった瞬間に動く部屋
    void Start()
    {
        flipCount = 0;
        timer = 0f;// 新しくゲームを始めるたびに、記録を0にリセットする！

        // 自分に付いているスピーカー（AudioSource）を自動で見つけて持ってくる
        audioSource = GetComponent<AudioSource>();

        ShuffleAndAssignCards();
        UpdateScoreText();
    }

    void Update()
    {
        if (isReady == true) // ⭐️【カウントダウン中（最初の3秒間）】
        {
            startDelayTimer += Time.deltaTime;
            int displayCount = (int)(3.0f - startDelayTimer + 1.0f);

            // 💡【修正：順番を一番上に！】数字が変わった「その瞬間」、文字が変わるより前に音を鳴らす！
            if (displayCount != lastDisplayedCount && displayCount >= 1 && displayCount <= 3)
            {
                lastDisplayedCount = displayCount;

                // ⭐️【裏技】再生の前に一瞬だけ音を「0.01秒でいいから準備」させてから鳴らす
                // これにより音の再生エンジンが「あ、次鳴るな」と準備して遅延を消します
                audioSource.clip = countdownSound;
                audioSource.Play();
            }

            readyText.text = displayCount.ToString();// ❶ 真ん中のデカ文字に「3、2、1」を表示

            timerText.text = "のこり: 60.0秒";// ❷ 左上のタイマーにはゲーム開始前は「のこり: 60.0秒」で止めておく

            if (startDelayTimer >= 3.0f)
            {
                isReady = false; // 本編スタート！

                // 💡【修正：これもデカ文字を消す前に！】「Go!」の音を鳴らす！
                PlaySound(startVoiceSound);

                readyText.gameObject.SetActive(false); // ⭐️【新設】用が済んだので真ん中のデカ文字を消す！
            }
            return;
        }

        // ❶ もしすでにゲームクリアしているなら、これ以上下の処理は何もせずにここで終わり！
        if (isGameClear == true) return;

        // （※ここから下のタイマーの処理はそのままです！）
        timer += Time.deltaTime;
        float remainingTime = 60.0f - timer;
        if (remainingTime < 0f) remainingTime = 0f;
        timerText.text = "のこり: " + remainingTime.ToString("F1") + "秒";

        if (timer >= 60.0f)
        {
            isGameClear = true;
            Debug.Log("タイムアップ！ゲームオーバー！");
            //PlaySound(gameOverSound);
            UnityEngine.SceneManagement.SceneManager.LoadScene("GameOverScene");
        }
    }

    // カードを集めて、シャッフルして、数字を配る魔法のメソッド
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

                // 💡ここで正解音（ティロリーン！）を即鳴らす！
                PlaySound(correctSound);

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

                // 💡ここで不正解音（ブブー）を即鳴らす！
                PlaySound(incorrectSound);

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

            // クリア画面に飛ぶ前にクリア音を鳴らす！
            //PlaySound(clearSound);

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

    // 音を安全に鳴らすための命令
    private void PlaySound(AudioClip clip)
    {
        if (audioSource != null && clip != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }
}