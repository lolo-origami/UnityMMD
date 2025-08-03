# LLToon概要

トゥーンシェーダーは、より柔軟でイラスト表現を目指した様々な機能を持つオリジナルのシェーダーです。
基本のトゥーンライティングにどんどん光を加算する、という思想で作成されています。暗部での情報量を増やしやすいのが特徴です。
ForwardLightingをベースに、追加光やUnityのLightProbeにも対応しています。

## Toon機能

### 基本(Diffuse)

- **2段階の影設定**: 「1影」と「2影」の2段階で影を調整します。
  - **ShadowMultColor**: 1影の色を調整します。
  - **ShadowArea**: 1影の範囲を調整します。
  - **ShadowSmooth**: 1影の境界線の滑らかさを調整します。
  - **Use DarkShadow**: 2影を有効にするか設定します。
  - **DarkShadowMultColor**: 2影の色を調整します。
  - **DarkShadowArea**: 2影の範囲を調整します。
  - **DarkShadowSmooth**: 2影の境界線の滑らかさを調整します。

### 追加機能 
- **2影反転による照り返し表現**: 通常の影とは逆の方向に2影を生成し、特殊なライティング効果を作り出すことができます。
  - **InverseDarkShadow**: 逆の2影を有効にするか設定します。
  - <img width="317" height="201" alt="image" src="https://github.com/user-attachments/assets/f7391556-1d41-4ae9-952e-78ce1cbf7f61" />


- **固定されたライトのY軸**: ライトのY軸の値を固定し、常に一定の高さから光が当たっているような表現が可能です。主に顔に対して使われます。
  - **Use FixLightY**: ライトのY軸を無視するか設定します。
  - <img width="209" height="219" alt="image" src="https://github.com/user-attachments/assets/0909a9e7-21ef-43d0-8366-54ed1c63e491" /> <img width="265" height="219" alt="image" src="https://github.com/user-attachments/assets/b630ba78-2ec9-426e-af6e-8645c7f92dbc" />

- **マテリアルごとの影の色**: マスクテクスチャのAチャンネルを利用して、一つのマテリアル内で影の色を2種類使い分けることが可能です。服と肌が同一マテリアルの時等に利用ください。
  - **SceondMaterialShadowColor**: 2つ目のマテリアル用の1影の色を調整します。
  - **SceondMaterialDarkShadowColor**: 2つ目のマテリアル用の2影の色を調整します。
    
- **影色ブレンド**: 影部専用の色情報に切り替えます。シーンでのボリューム設定やTimeline等と組み合わせて活用ください。ベースカラーが1影、1影が1影と2影のブレンド、2影の色が弱まるように設定されています。これは、影の中に入った時にコントラストが弱くなる効果を想定しています。
  - **OnShadowForChara**: 影色ブレンドをオンにします。 

* **EnableFaceCheek**: 顔のシェーディングを有効にするか設定します。

## ハイライト(Specular)関連

トゥーンシェーディングをベースにしつつ、BRDFモデルを使ったハイライト表現を追加することができます。

### 基本
- **EnableSpecular**: スペキュラを有効にするか設定します。
- **LightSpecColor**: スペキュラの色を設定します。
- **BRDF表現**
  - **Metallic**: 金属感を調整します。
  - **Smoothness**: 滑らかさを調整します。
- **スペキュラタイプ切り替え**: `EnableHair`を有効にすることで、髪の毛に異方性反射を元にしたスペキュラを表現できます。
  - **EnableHair**: 髪の毛に特化したスペキュラを有効にするか設定します。
   - **Sharpness**: 髪の毛用スペキュラの鋭さを調整します。
  - **EnableMatCapSpecular (`_EnableMatCapSpecular`)**: Matcapによるスペキュラを有効にするか設定します
    - **MatCapIntensity**: Matcapの強度を調整します。

### 追加機能
- **高輝度スぺキュラ**: より鋭く出したいハイライトを描画できます。
 - **SpecularHighIntensity**: 高強度のスペキュラの強度を調整します。
- **影内ハイライト機能**： 影の中に入っても、ハイライトを描画できます。
  - **ShadowHighlightColor**: 影部分のハイライト色を設定します。
  - **SpecularIntensityShadow**: 影部分のスペキュラの強度を調整します。

## 輝き制御機能(BloomInputs)
Emissionや、リムライト等を制御するための機能です。Specularやベースの色の強さもまとめて制御できます。
### Emission
- **EnableEmission**: エミッションを有効にするか設定します。
- **EmissionColor**: エミッションの色を調整します。
- **EmissionIntensity**: エミッションの強度を調整します。
- **EmissionBloomFactor**: エミッションのブルーム係数を調整します。
- **DarkEmissionIntensity**: 暗部のエミッション強度を調整します。

### リムライト
- **EnableRim**: リムライトを有効にするか設定します。
- **RimColor**: リムライトの色を調整します。
- **Rim Smooth**: リムライトの境界線の滑らかさを調整します。
- **Rim Pow**: リムライトの広がりを調整します。 

#### 追加機能 
- **BlendRimWithBaseColor**: リムライトの色をベースカラーと馴染ませるかどうかの切り替えを設定します。
- **EnableLambertRim**: ライトが当たっている明るい部分ではリムライトが強く出て、影になっている暗い部分ではリムライトが弱くなるように計算されるようになります。
- **EnableDarkRim**: 暗い部分のリムライトを有効にするか設定します。
  - **DarkRimColor**: 暗い部分のリムライトの色を調整します。
  - **Dark Rim Smooth**: 暗い部分のリムライトの強度を調整します。
  - **Rim Pow**: 暗い部分のリムライトの広がりを調整します。

### 全体調整
- **DiffuseIntensity**: ディフューズの強度を調整します。
- **SpecularIntensity**: スペキュラの強度を調整します。
- **WorldLightInfluence**: メインのディレクショナルライトの影響度を調整します。
- **GIInfluence**: グローバルイルミネーション（GI）の総合的な影響度を調整します。
- **AddLightInfluence**: 追加ライトの影響度を調整します。
- **LightMapInfluence**: ライトマップの影響度を調整します。ベイクされたGIに影響します。
- **BloomRimSpecFactor**: ハイライト、リムライト、エミッションを総合的に調整します。最終的なグローの調整に使ってください


### Outline
UniToonをベースにしたアウトライン制御です。

- **OutlineMask**: アウトラインの表示/非表示を制御するマスクテクスチャです。
- **OutlineWidth**: アウトラインの太さを調整します。
- **OutlineLightAffects**: アウトラインのライティング影響度を調整します。
- **OutlineSaturation**: アウトラインの彩度を調整します。
- **OutlineBrightness**: アウトラインの明るさを調整します。
- **OutlineStrength**: アウトラインの強さを調整します。
- **OutlineSmoothness**: アウトラインの滑らかさを調整します。
