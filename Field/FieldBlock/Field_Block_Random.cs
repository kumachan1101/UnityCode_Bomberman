using UnityEngine;

public class Field_Block_Random : Field_Block_CpuMode {
    protected override void SetFieldRange() {
        // ランダムなフィールドサイズを設定
        int width = Random.Range(8, 15); // 8から15の範囲でランダムに幅を決定
        int height = Random.Range(8, 15); // 8から15の範囲でランダムに高さを決定
        GameManager.SetFieldRange(width, height);
    }

}