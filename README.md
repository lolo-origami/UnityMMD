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

# LLPostProcess概要

独自のVolumePostProcessです。
URPのRendererに使いたいPostProcessを登録してください。
RenderFeatureの登録順で実行順が自動入れ替わります。

<img width="460" height="110" alt="image" src="https://github.com/user-attachments/assets/8a432cd5-2d1f-4111-a3f5-4b8b5be3ea85" />


## DepthFog

カメラの Depth Textureを元にした距離依存のフォグ使います。

<img width="312" height="559" alt="image" src="https://github.com/user-attachments/assets/33f6f876-36e7-47d5-9fe8-fb4f20d45397" /> <img width="312" height="559" alt="image" src="https://github.com/user-attachments/assets/99ed65f0-464d-468c-a292-5ff353b90af5" />

<img width="458" height="97" alt="image" src="https://github.com/user-attachments/assets/a26a83e3-25a8-4ac0-a87a-207f06586f2e" />

- **FogColor**: フォグの色。
- **Intensity**: Fogの強さ。

## DepthLightShaft

カメラの Depth Textureを元にしたゴッドレイ効果を出力します。ブラー効果のみで軽量に動作することが可能です。



## Flare

アニメやイラストで加工に使われる、特定箇所へのグラデーション設定です。

## Diffusion

<img width="352" height="628" alt="image" src="https://github.com/user-attachments/assets/d13c8c09-a9ac-4477-ac81-d761ceb3d2d4" /> <img width="353" height="628" alt="image" src="https://github.com/user-attachments/assets/ab0f6cdf-1b2f-4f0b-9556-557023c2f30e" />

<img width="509" height="176" alt="image" src="https://github.com/user-attachments/assets/424d6a6c-9c6e-443e-b67f-8b2e11226c22" />



光が拡散するような画像加工を行います。
コントラストを調整し、縦横のブラーをかけた後に合成を行います。
合成は、加算、スクリーン、覆い焼きカラー、比較(明)が選べます。

- **Contrast**:合成前の元画像のコントラストを強める  
- **BlurSize**:ぼかしの強さ  
- **BlendMode**:合成モード
- **BlendIntensity**:合成の強さ  

## SSRaymarchLightShaft

<img width="309" height="553" alt="image" src="https://github.com/user-attachments/assets/7ba09c8a-30b0-42ea-882d-4e26b5fc2add" /> <img width="309" height="554" alt="image" src="https://github.com/user-attachments/assets/58afbe7d-6203-4b13-a174-7ee312af5730" />

<img width="462" height="362" alt="image" src="https://github.com/user-attachments/assets/c4fe836e-5d11-49ec-9372-ff89ff1b83c5" />


スクリーンスペースでレイマーチ処理を行ってゴッドレイ効果を出すものです。
遮蔽物が画面内にあれば、その後ろで光がさえぎられている効果を足せます。レイマーチなので重たいです。
複数の合成モードを選択できます。

- **RayColor**: レイの基本色。距離に応じてシーンカラーとブレンドされる色。  
- **RayIntensity**：レイの強さ
- **MaxIterations**：サンプリング回数。大きくするとフォグの精度が上がるが、処理コストも増える。  
- **MinDistance**:カメラからレイマーチ開始距離。
- **MaxDistance**:レイマーチ終了距離。  
- **Decay**:レイを減衰させる係数。1.0 で減衰なし、0.9 などで徐々に薄れていく。  
- **FalloffPower**:密度変化を制御するカーブ。小さい値で均一、大きくすると距離に応じて急激に濃くなる。  
- **JitterStrength**:サンプリング位置にランダムな揺らぎを与える。  
- **NoiseScale**:ノイズテクスチャのスケール。
- **BlendMode**:合成モード
- **BlendIntensity**:合成の強さ  

## SSRaymarchFog

<img width="313" height="552" alt="image" src="https://github.com/user-attachments/assets/2e3df06f-9bf0-4a64-9ec2-f272ada1b4d1" /> <img width="309" height="549" alt="image" src="https://github.com/user-attachments/assets/306691fc-99e2-4f3d-b4fb-4400501c2045" />

<img width="469" height="204" alt="image" src="https://github.com/user-attachments/assets/6c108260-7b4d-4721-a9c3-ebd62ca3b69e" />

スクリーンスペースでレイマーチ処理を行ってフォグ効果を出すものです。
Depthだけで行うフォグよりも、遮蔽物とフォグがかかる箇所がくっきりしています。レイマーチなので重たいです。
複数の合成モードを選択できます。

- **FogColor**：フォグの色
- **FogIntensity**：フォグの強さ
- **MaxIterations**：サンプリング回数。大きくするとフォグの精度が上がるが、処理コストも増える。  
- **MinDistance**:カメラからレイマーチ開始距離。
- **MaxDistance**:レイマーチ終了距離。
- **BlendMode**:合成モード
- **BlendIntensity**:合成の強さ   

## GuidedFilter
<img width="313" height="553" alt="image" src="https://github.com/user-attachments/assets/4f25f0c2-c216-4a3e-a18e-b7c5367abd9c" /> <img width="313" height="552" alt="image" src="https://github.com/user-attachments/assets/3a83eb83-190b-45bc-9732-1f5a91723b95" />

<img width="472" height="199" alt="image" src="https://github.com/user-attachments/assets/78f46285-4f45-45e8-a392-41a5b92b3ca3" />

ガイド画像を用いてエッジを保持しつつ平滑化するフィルタ。
ガイド画像には「入力カラー（Self）」「深度（Depth）」「外部テクスチャ（Other）」を選択可能。
単純なブラーと異なり、エッジ付近の構造を保ちながら滑らかにすることができる。

- **Radius**：フィルタの半径。大きくするとより広範囲が平滑化されるが、処理コストも増加します。
- **Eps**：正則化パラメータ。小さい値にすると入力画像に忠実、大きくすると平滑化が強くなります。
- **GuideMode**：ガイド画像の種類を選択します。
  - Self：入力カラーそのものをガイドに利用。色味をなるべく保ちながら平滑化。
  - Depth：カメラ深度を二値化したマスクをガイドに利用。奥行きに沿ってフォグ・マスク的な平滑化を実現。
  - Other：外部に指定したテクスチャをガイドに利用。マスク画像や別のレンダリング結果を活用可能。
- **GuideTex**：外部ガイド用テクスチャ。GuideMode=Other のときのみ利用。
- **DepthThreshold**：Depthモード時のしきい値。カメラからの深度を基準に、手前と奥を分ける境界を決定します。
- **DepthFeather**：Depthモード時のフェザー幅。しきい値付近をなめらかに補間することで、エッジのギザつきを防ぎます。
- **DepthColorBlend**：Depthモード時のみ有効。フィルタ結果 q と元カラー p をブレンドする割合。

## AnimeToneMap

アニメ的な色彩、彩度とコントラストを保持しながら、必要に応じてガンマ補正やハイライト圧縮を行います。。

黒の持ち上げ
<img width="351" height="628" alt="image" src="https://github.com/user-attachments/assets/466bbb25-725a-4dfc-ba52-08f8c63869a0" />

<img width="359" height="630" alt="image" src="https://github.com/user-attachments/assets/86049d75-f78b-44e7-86a7-b18b9865a37b" />

白飛びの軽減

<img width="352" height="628" alt="image" src="https://github.com/user-attachments/assets/ca9fca90-877d-4542-a30f-35a1ba7268b3" />

<img width="351" height="628" alt="image" src="https://github.com/user-attachments/assets/22777c8f-6cdd-445e-ba1e-6680a1565563" />


Unity標準のACES

<img width="352" height="626" alt="image" src="https://github.com/user-attachments/assets/9b470c9b-010d-4221-b9fd-1f7386d670fd" />

<img width="354" height="624" alt="image" src="https://github.com/user-attachments/assets/6ed8b78f-1fd1-426e-bea4-53ae7f26b118" />

- **Gamma**：ガンマ補正で明暗を強調します。
- **Contrast**：コントラストを強め、ベタ塗り感を保ちます。
- **Saturation**：彩度を調整し、鮮やかさを維持します。
- **HighlightCompress**：明部のみに適用される圧縮カーブで、白飛びを防ぎます。
- **WhitePoint**：圧縮カーブを適用する輝度値





