using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//총알 발사와 재장전 오디오 클립을 저장할 구조체
[System.Serializable]
public struct PlayerSfx
{
    public AudioClip[] fire;
    public AudioClip[] reload;
    public AudioClip hit;
}

public class FireCtrl : MonoBehaviour
{
    //무기타입
    public enum WeaponType
    {
        RIFLE=0,SHOTGUN
    }

    //주인공이 현재 들고 있는 무기를 저장할 변수
    public WeaponType currWeapon = WeaponType.RIFLE;
    //총알 프리팹
    public GameObject bullet;
    //총알 발사 좌표
    public Transform firePos;
    //탄피 추출 파티클
    public ParticleSystem cartridge;
    //AudioSource 컴포넌트를 저장할 변수
    AudioSource _audio;

    //총구 화염 파티클
    ParticleSystem muzzleFlash;
    //오디오 클립을 저장할 변수
    public PlayerSfx playerSfx;

    //발사 여부 판단
    bool isFire = false;
    //발사 간격
    float fireRate = 0.1f;
    //장전
    bool reloading = false;
    //남은 총알
    int remainingBullet=1;
    //최대 총알
    int maxBullet = 30;
    //다음 발사 시간 저장
    float nextFire;

    //남은 총알수
    public Text magzineText;
    //재장전 시간
    public float reloadTime = 2f;


    //Shack 클래스 저장
    //public Shack shack;
   // Shack shake;
    void Start()
    {
        //FirePos 하위에 있는 컴포넌트 추출
        muzzleFlash = firePos.GetComponentInChildren<ParticleSystem>();
        //AudioSource 컴포넌트 추출
        _audio = GetComponent<AudioSource>();
        //Shack 스크립트 추출
        //shake = GetComponent<Shack>();
    }

    void Update()
    {
        //마우스 왼쪽 버튼을 클릭했을 때 Fire 함수 호출
        if (!reloading && Input.GetMouseButton(0))
        {           
                if (Time.time>nextFire)
                {
                    --remainingBullet;
                    Fire();
                    //남은 총알이 없을 경우 재장전
                    if (remainingBullet == 0)
                    {
                        StartCoroutine(Reloading());
                        GetComponent<PlayerCtrl>().Playergunreload();
                    }
                    //다음 총알 발사시간
                    nextFire = Time.time + fireRate;
                }            
        }
    }

    IEnumerator Reloading()
    {
        reloading = true;
        _audio.PlayOneShot(playerSfx.reload[(int)currWeapon], 1f);
        //재장전 오디오의 길이
        yield return new WaitForSeconds(playerSfx.reload[(int)currWeapon].length + 0.3f);
        //각종 변수값의 초기화
        reloading = false;
        remainingBullet = maxBullet;
        //남은 총알수 갱신
        UpdateBulletText();
    }

    void UpdateBulletText()
    {
        //(남은 총알 수/ 최대 총알수) 
        magzineText.text = string.Format("<color=#ff000>{0}</color>/{1}", remainingBullet, maxBullet);
    }

    void Fire()
    {
        //셰이크 효과 호출
       // shake.Hitted();
       
        //StartCoroutine(shack.ShackCamera());
        //Bullet 프리팹
        //Instantiate(bullet, firePos.position, firePos.rotation);
        var _bullet = GameManager.instance.GetBullet();
        if (_bullet != null)
        {
            _bullet.transform.position = firePos.position;
            _bullet.transform.rotation = firePos.rotation;
            _bullet.SetActive(true);
        }
        cartridge.Play();
        muzzleFlash.Play();
        FireSfx();
        //갱신
        UpdateBulletText();
    }

    //적한테 공격 당했을 때 실행할 함수 
    public void HitDam()
    {
        var _sfx=playerSfx.hit;
        _audio.PlayOneShot(_sfx, 1f);
    }

    void FireSfx()
    {
        //현재 들고 있는 무기의 오디오 클립을 가져옴
        var _sfx = playerSfx.fire[(int)currWeapon];
        //사운드 발생
        _audio.PlayOneShot(_sfx, 1f);
    }

}
