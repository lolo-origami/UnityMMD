### LLToon概要

トゥーンシェーダーは、より柔軟でイラスト表現を目指した様々な機能を持つオリジナルのシェーダーです。

#### Toon機能

* **2段階の影設定**: 「1影」と「2影」の2段階で影を表現できます。
* **影の範囲と境界線の調整**: それぞれの影の適用範囲と境界線の滑らかさを個別に制御できます。
* **反転した2影**: 通常の影とは逆の方向に2影を生成し、特殊なライティング効果を作り出すことができます。
* **固定されたライトのY軸**: ライトのY軸の値を固定し、常に一定の高さから光が当たっているような表現が可能です。
* **マテリアルごとの影の色**: マスクテクスチャのAチャンネルを利用して、一つのマテリアル内で影の色を2種類使い分けることが可能です。
* * **影内でのライティング**: 影に入った際に、自動で影部専用のライティングに切り替えます
のまとめ

##### ToonShader Inputs

* **ShadowMultColor**: 1影の色を調整します。
* **SceondMaterialShadowColor**: 2つ目のマテリアル用の1影の色を調整します。
* **SceondMaterialDarkShadowColor**: 2つ目のマテリアル用の2影の色を調整します。
* **ShadowArea**: 1影の範囲を調整します。
* **ShadowSmooth**: 1影の境界線の滑らかさを調整します。
* **Use DarkShadow**: 2影を有効にするか設定します。
* **DarkShadowMultColor**: 2影の色を調整します。
* **DarkShadowArea**: 2影の範囲を調整します。
* **DarkShadowSmooth**: 2影の境界線の滑らかさを調整します。
* **InverseDarkShadow**: 逆の2影を有効にするか設定します。
* **Use FixLightY**: ライトのY軸を無視するか設定します。
* **OnShadowForChara**: 影の中でのキャラクターシェーディングを有効にするか設定します。
* **EnableFaceCheek (`_EnableFaceCheek`)**: 顔のシェーディングを有効にするか設定します。

### ハイライト(Specular)関連

トゥーンシェーディングをベースにしつつ、物理ベースレンダリングの概念を取り入れたBRDF表現を追加することができます。

#### 追加機能説明

* **髪のスペキュラ**: `EnableHair`を有効にすることで、髪の毛に特有のスペキュラを表現できます。
* **影部分のハイライト**: メインライトの影になっている部分のハイライトの色や強度を個別に設定できます。
* **Matcapスペキュラ**: `EnableMatCapSpecular`を有効にすることで、Matcapテクスチャを使ったスペキュラ表現が可能になります。
* **顔のシェーディング**: `EnableFaceCheek`を有効にすることで、顔に特化したシェーディングを適用できます。
* **高輝度スぺキュラ**: より鋭く出したいハイライトを描画できます。

#### 詳細パラメータのまとめ

##### BRDF Inputs

* **Metallic (`_Metallic`)**: 金属感を調整します。
* **Smoothness (`_Smoothness`)**: 滑らかさを調整します。
* **EnableSpecular (`_EnableSpecular`)**: スペキュラを有効にするか設定します。
* **LightSpecColor (`_LightSpecColor`)**: スペキュラの色を設定します。
* **ShadowHighlightColor (`_LightSpecShadowColor`)**: 影部分のハイライト色を設定します。
* **EnableHair (`_EnableHairSpecular`)**: 髪の毛に特化したスペキュラを有効にするか設定します。
* **Sharpness (`_Sharpness`)**: 髪の毛用スペキュラの鋭さを調整します。
* **SpecularIntensity (`_SpecularIntensity`)**: スペキュラの強度を調整します。
* **SpecularHighIntensity (`_SpecularIntensityHigh`)**: 高強度のスペキュラの強度を調整します。
* **SpecularIntensityShadow (`_SpecularIntensityShadow`)**: 影部分のスペキュラの強度を調整します。
* **MatCapIntensity (`_MatCapIntensity`)**: Matcapの強度を調整します。
* **EnableMatCapSpecular (`_EnableMatCapSpecular`)**: Matcapによるスペキュラを有効にするか設定します。

---

### Bloom機能

オブジェクトの発光と、それに伴うブルーム効果を制御するための機能です。

#### 機能羅列と簡易な説明

* **エミッション**: オブジェクト自体が発光しているように見せる機能です。
* **ブルームとリムライトの連携**: エミッションやリムライトの輝きをブルーム効果に反映させることができます。
* **暗部のエミッション強度**: 暗部でのエミッション強度を個別に調整できます。

#### 詳細パラメータのまとめ

##### Bloom Inputs

* **DiffuseIntensity (`_DiffuseIntensity`)**: ディフューズの強度を調整します。
* **WorldLightInfluence (`_WorldLightInfluence`)**: ワールドライトの影響度を調整します。
* **GIInfluence (`_GIInfluence`)**: グローバルイルミネーション（GI）の影響度を調整します。
* **AddLightInfluence (`_AddLightIntensity`)**: 追加ライトの影響度を調整します。
* **LightMapInfluence (`_LightMapInfluence`)**: ライトマップの影響度を調整します。
* **BloomRimSpecFactor (`_BloomFactor`)**: ブルームの共通係数を調整します。
* **EnableEmission (`_EnableEmission`)**: エミッションを有効にするか設定します。
* **EnableRim (`_EnableRim`)**: リムライトを有効にするか設定します。
* **EnableDarkRim (`_EnableDarkRim`)**: 暗い部分のリムライトを有効にするか設定します。
