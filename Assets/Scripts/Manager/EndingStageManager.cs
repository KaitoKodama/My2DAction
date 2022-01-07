using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class EndingStageManager : MonoBehaviour
{
	[SerializeField] GameObject actorCanvas;
	[SerializeField] AudioSource bgmAudioSource;
	[SerializeField] AudioClip textSound;
	[SerializeField] RollData rollData;
	[SerializeField] GameObject storyPanel;
	[SerializeField] CanvasGroup storyGroup;

	[SerializeField] Button nextButton;
	[SerializeField] Image sideImage;
	[SerializeField] Text contentText;
	[SerializeField] Sprite actorSprite;
	[SerializeField] Sprite enemySprite;

	enum Side { Actor, Enemy, }
	private List<AnyDictionary<Side, string>> messegeDics;
	private Tween tween;
	private AudioSource audioSource;
	private int messegeIndex = 0;

	private void Start()
	{
		#region 会話内容の登録
		messegeDics = new List<AnyDictionary<Side, string>>()
		{
			new AnyDictionary<Side, string>(Side.Enemy, "人間、いや精霊使いを見たのは久しぶりだな"),
			new AnyDictionary<Side, string>(Side.Actor, "だれ？！"),
			new AnyDictionary<Side, string>(Side.Enemy, "君たちが穢れという存在だよ"),
			new AnyDictionary<Side, string>(Side.Actor, "あなたがおばあちゃんの言っていた「たった一匹の穢れ」なの？"),
			new AnyDictionary<Side, string>(Side.Enemy, "私は、そんな風に呼ばれていたのか"),
			new AnyDictionary<Side, string>(Side.Enemy, "そうとも、私は唯一知識を持つ穢れ「たった一匹の穢れ」と呼ばれるものだ"),
			new AnyDictionary<Side, string>(Side.Enemy, "精霊使いよ、君の名はなんというのかね？"),
			new AnyDictionary<Side, string>(Side.Enemy, "そして、なぜここまでやってきたのだ？"),
			new AnyDictionary<Side, string>(Side.Actor, "私はイーロイ、好奇心.... かな？"),
			new AnyDictionary<Side, string>(Side.Enemy, "はっはっはっ！なんとも人間らしい！"),
			new AnyDictionary<Side, string>(Side.Enemy, "世界の成り立ちを知りたいのか！"),
			new AnyDictionary<Side, string>(Side.Actor, "教えてくれる？？"),
			new AnyDictionary<Side, string>(Side.Enemy, "よいとも、しかしここまでたどり着いたということは"),
			new AnyDictionary<Side, string>(Side.Enemy, "この話を聞いた後、君は罪を背負わなければならなくなる"),
			new AnyDictionary<Side, string>(Side.Enemy, "それでも聞くか？"),
			new AnyDictionary<Side, string>(Side.Actor, "それでも知りたい"),
			new AnyDictionary<Side, string>(Side.Enemy, "よかろう、すべて教えよう"),

			new AnyDictionary<Side, string>(Side.Enemy, "1000年前、人は世界中で大繁栄をしていた"),
			new AnyDictionary<Side, string>(Side.Enemy, "しかし、人は世界を汚染し"),
			new AnyDictionary<Side, string>(Side.Enemy, "地上はガスマスク無しでは生きられない場所になってしまった"),

			new AnyDictionary<Side, string>(Side.Enemy, "200年後、世界は貧困層"),
			new AnyDictionary<Side, string>(Side.Enemy, "持たざる者たちを被験者とし、遺伝子操作を開始"),
			new AnyDictionary<Side, string>(Side.Enemy, "しかし、遺伝子操作が成功することはなく様々な奇怪な生命がそこに誕生した"),
			new AnyDictionary<Side, string>(Side.Enemy, "我々穢れもその奇怪な生命のひとつだ"),

			new AnyDictionary<Side, string>(Side.Enemy, "それから100年後"),
			new AnyDictionary<Side, string>(Side.Enemy, "人間は徐々に弱体化していき、技術の大半が失われかけていた"),
			new AnyDictionary<Side, string>(Side.Enemy, "しかしそこで初めて、遺伝子操作が成功"),
			new AnyDictionary<Side, string>(Side.Enemy, "それが君たち精霊使いだ"),

			new AnyDictionary<Side, string>(Side.Actor, "私はハラカラを殺し続けてしまったのね ..."),
			new AnyDictionary<Side, string>(Side.Actor, "なんてことを ..."),
			new AnyDictionary<Side, string>(Side.Actor, "これが私の罪 ..."),

			new AnyDictionary<Side, string>(Side.Enemy, "いいや、これは君の罪ではない"),
			new AnyDictionary<Side, string>(Side.Actor, "えっ！？"),

			new AnyDictionary<Side, string>(Side.Enemy, "君は穢れを殺した、ハラカラではない"),
			new AnyDictionary<Side, string>(Side.Enemy, "そもそもハラカラを殺すのは人間にとって日常ようなもの"),
			new AnyDictionary<Side, string>(Side.Actor, "....."),
			new AnyDictionary<Side, string>(Side.Enemy, "穢れが毒を吐く理由を教えよう"),
			new AnyDictionary<Side, string>(Side.Enemy, "それこそが君の罪だ"),

			new AnyDictionary<Side, string>(Side.Enemy, "君は道中綺麗な自然を見たはずだ"),
			new AnyDictionary<Side, string>(Side.Enemy, "なぜ、汚染されたはずの自然が美しいのか？"),
			new AnyDictionary<Side, string>(Side.Enemy, "それは、我々がそれを食らうからだ"),
			new AnyDictionary<Side, string>(Side.Enemy, "そして毒を吐く、しかしその毒はかなり純度が落ちている"),
			new AnyDictionary<Side, string>(Side.Enemy, "何百年と続けるうちに地上から汚染は減り、世界は元の姿を取り戻そうとしている"),
			new AnyDictionary<Side, string>(Side.Enemy, "しかし君は、我々のほとんどを殺した"),
			new AnyDictionary<Side, string>(Side.Enemy, "世界は元の姿を手に入れられなくなってしまったのだ"),
		};
		#endregion

		nextButton.onClick.AddListener(() => { OnNextButton(); });
		audioSource = GetComponent<AudioSource>();
	}
	
	public void StartEndingScene()
	{
		bgmAudioSource.DOFade(0.1f, 5f);
		actorCanvas.SetActive(false);
		GameObject.FindWithTag("Player").GetComponent<Actor>().enabled = false;
		storyPanel.SetActive(true);
		storyGroup.DOFade(1, 1f).OnComplete(() =>
		{
			OnNextButton();
		});
	}

	private void OnNextButton()
	{
		if(messegeIndex >= messegeDics.Count)
		{
			GameManager.instance.OnSceneTransition(StageNames.TitleScene, rollData);
		}
		else
		{
			tween?.Kill(true);
			audioSource.PlayOneShot(textSound);
			contentText.text = "";
			tween = contentText.DOText(messegeDics[messegeIndex].value, 0.5f).SetEase(Ease.Linear);

			switch (messegeDics[messegeIndex].key)
			{
				case Side.Actor:
					sideImage.sprite = actorSprite;
					break;
				case Side.Enemy:
					sideImage.sprite = enemySprite;
					break;
			}
			messegeIndex++;
		}
	}
}
