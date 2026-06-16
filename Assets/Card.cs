using UnityEngine;
using TMPro; // 👈 綺麗な文字（TextMeshPro）をプログラムでいじるための準備

public class Card : MonoBehaviour
{
    // カードの「正解の数字」を入れておく箱
    public int cardNumber;

    // ボタンの中に隠れている文字（TextMeshPro）のコンポーネントを入れておく箱
    private TextMeshProUGUI cardText;

    void Start()
    {
        // ゲームが始まったら、自分の下にある文字（Text）の部品を自動で見つけて持ってくる
        cardText = GetComponentInChildren<TextMeshProUGUI>();
    }

    // ボタンがクリックされたときに実行される
    public void OnClickCard()
    {
        // クリックされたら、文字を「？」から「自分の数字」に書き換える！
        cardText.text = cardNumber.ToString();

        // 📢【新しく追加する行】画面にいるGameManagerを探して、「めくられたよ！」と報告する
        //FindObjectOfType<GameManager>().CardFlipped(this);
        // 〇 新しい書き方（これに書き換える！）
        Object.FindFirstObjectByType<GameManager>().CardFlipped(this);

        Debug.Log("めくったカードの数字は: " + cardNumber);

    }// ちゃんとここでOnClickCardのお部屋が終了！

    // ⭐️【独立した新しいお部屋】レフェリーから呼ばれたら、文字を「？」に戻す
    public void HideCard()
    {
        cardText.text = "?";
    } // HideCardのお部屋終了！

}