using UnityEngine;
using TMPro; // 綺麗な文字（TextMeshPro）をプログラムでいじるための準備
using System.Collections;

public class Card : MonoBehaviour
{
    public int cardNumber;// カードの「正解の数字」を入れておく箱
    private TextMeshProUGUI cardText;// ボタンの中に隠れている文字（TextMeshPro）のコンポーネントを入れておく箱
    [SerializeField] private Color defaultColor = Color.white;  // 裏面（めくる前）の色
    [SerializeField] private Color flippedColor = Color.yellow; // 表面（めくった後）の色

    private UnityEngine.UI.Image cardImage; // カードの見た目（色）を変えるためのコンポーネントを入れる箱
  
    private AudioSource audioSource;// 音を鳴らすためのコンポーネント（スピーカー）を入れておく箱
    
    [SerializeField] private AudioClip flipSound;// めくったときの音（効果音データ）を入れておく箱

    void Start()
    {
        cardImage = GetComponent<UnityEngine.UI.Image>(); // 自分のImageコンポーネントを取得する
        cardImage.color = defaultColor;// ⭐️ 最初は「裏面の色」にしておく

        cardText = GetComponentInChildren<TextMeshProUGUI>();// ゲームが始まったら、自分の下にある文字（Text）の部品を自動で見つけて持ってくる
        audioSource = GetComponent<AudioSource>();// 自分に付いているスピーカー（AudioSource）を自動で見つけて持ってくる
    }

    // ボタンがクリックされたときに実行される
    public void OnClickCard()
    {

        // 画面のレフェリー（GameManager）を一旦変数に入れておく
        GameManager gm = Object.FindFirstObjectByType<GameManager>();

        // ゲームがまだ「開始前（のこり60秒で停止中）」なら無視して帰る！
        if (gm != null && gm.timerText.text.Contains("60.0秒"))
        {
            return;
        }

        // 画面のレフェリーを探して、「いまロック中？」と聞く
        if (gm != null && gm.IsLocking == true)
        {
            return; // もしロック中（true）だったら、この下の処理を何もせずに「帰れ（return）」と命じる！
        }

        if (cardText.text != "?") // もし、今の文字が「?」じゃないなら（すでに数字がめくられているなら）無視して帰る！
        {
            return;
        }

        // もし次が「2枚目」じゃないとき（＝いま1枚目をめくろうとしているとき）だけ、めくる音を鳴らす！
        if (gm != null && gm.FlippedCount == 0)
        {
            if (audioSource != null && flipSound != null)
            {
                audioSource.PlayOneShot(flipSound);
            }
        }

        cardImage.color = flippedColor; // 安全確認が「すべてセーフ！」とわかってから、初めて色を黄色にする！！！
        cardText.text = cardNumber.ToString(); // クリックされたら、文字を「？」から「自分の数字」に書き換える！

        if (gm != null)
        {
            gm.CardFlipped(this); // 画面にいるGameManagerを探して、「めくられたよ！」と報告する
        }
        Debug.Log("めくったカードの数字は: " + cardNumber);
    }

   
    public void HideCard()
    {
        cardText.text = "?"; // レフェリーから呼ばれたら、文字を「？」に戻す
        cardImage.color = defaultColor; // 色を裏面の色に戻してあげる！
    } 
    
    public void DeleteCard()// ⭐️【修正版】場所をズラさずに、見た目とボタンだけを消し去る
    {
        
        GetComponent<UnityEngine.UI.Button>().enabled = false;// ① ボタンの機能（クリックできる機能）をオフにする
        cardText.enabled = false;// ② 文字の部品（TextMeshPro）を非表示にして見えなくする
        GetComponent<UnityEngine.UI.Image>().enabled = false; // ③ ついでにボタンの背景（画像）も見えなくする
    }
    
    public void PopUpAnimation()// ペアが揃ったときに、可愛く弾けるアニメーションの窓口
    {
        StartCoroutine(PopUpCoroutine());// 1秒間のコルーチン（アニメーション）をスタートさせる
    }
    private IEnumerator PopUpCoroutine() // ⭐️【新設】アニメーションの本体（中身）
    {
        // 【演出の設定】0.2秒かけて、今のサイズの1.2倍まで大きくする設定
        float duration = 0.2f;
        float targetScale = 1.4f;
        Vector3 initialScale = Vector3.one; // 元のサイズ（1, 1, 1）

        float timer = 0f;

        while (timer < duration)
        {
            timer += Time.deltaTime;// Mathf.Lerp（ラープ）は、数字をなめらかに変化させる魔法の計算式！
            float scale = Mathf.Lerp(1.0f, targetScale, timer / duration);
            transform.localScale = initialScale * scale;
            yield return null; // 1フレーム待つ（これを入れないとフリーズするよ！）
        }
        // 2. 元のサイズに戻すアニメーション（ここを変えます！）
        // ⭐️【3.メリハリの改造】元に戻すのではなく、最高潮の大きさ（1.4倍）をそのまま維持する！
        transform.localScale = initialScale * targetScale; // targetScaleのままにする

        // ⭐️【4.間の改造】一番大きい状態で、0.1秒だけ「間」を置いて、プレイヤーに揃ったことをアピール！
        yield return new WaitForSeconds(0.1f); // (元は0.05秒)
    }

}