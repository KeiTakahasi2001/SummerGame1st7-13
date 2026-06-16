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

        // ⭐️【ここを追加！】画面のレフェリーを探して、「いまロック中？」と聞く
        // もしロック中（true）だったら、この下の処理を何もせずに「帰れ（return）」と命じる！
        if (Object.FindFirstObjectByType<GameManager>().IsLocking == true)
        {
            return;
        }

        // ⭐️【ここを新しく追加！】
        // もし、今の文字が「?」じゃないなら（すでに数字がめくられているなら）無視して帰る！
        if (cardText.text != "?")
        {
            return;
        }

        
        cardText.text = cardNumber.ToString();// クリックされたら、文字を「？」から「自分の数字」に書き換える！
        Object.FindFirstObjectByType<GameManager>().CardFlipped(this);// 📢【新しく追加する行】画面にいるGameManagerを探して、「めくられたよ！」と報告する
        Debug.Log("めくったカードの数字は: " + cardNumber);

    }// ちゃんとここでOnClickCardのお部屋が終了！

    // ⭐️【独立した新しいお部屋】レフェリーから呼ばれたら、文字を「？」に戻す
    public void HideCard()
    {
        cardText.text = "?";
    } // HideCardのお部屋終了！

    // ⭐️【新しく追加する命令】正解の時、レフェリーから呼ばれて画面から消える
    // ⭐️【修正版】場所をズラさずに、見た目とボタンだけを消し去る
    public void DeleteCard()
    {
        // ① ボタンの機能（クリックできる機能）をオフにする
        GetComponent<UnityEngine.UI.Button>().enabled = false;

        // ② 文字の部品（TextMeshPro）を非表示にして見えなくする
        cardText.enabled = false;

        // ③ ついでにボタンの背景（画像）も見えなくする
        GetComponent<UnityEngine.UI.Image>().enabled = false;
    }

}