# [Unity 3D] ShotShot
## 1. 소개

<div align="center">
  <img src="https://github.com/k660323/ShotShot/blob/main/Images/%EB%A9%94%EC%9D%B8%ED%99%94%EB%A9%B4.JPG" width="49%" height="300"/>
  <img src="https://github.com/k660323/ShotShot/blob/main/Images/%EC%98%B5%EC%85%98.JPG" width="49%" height="300"/>
  <img src="https://github.com/k660323/ShotShot/blob/main/Images/%EB%A7%8C%EB%93%A4%EA%B8%B0%20%EB%98%90%EB%8A%94%20%EC%B0%BE%EA%B8%B0.JPG" width="49%" height="300"/>
  <img src="https://github.com/k660323/ShotShot/blob/main/Images/%EB%A7%A4%EC%B9%AD%EC%A4%91.JPG" width="49%" height="300"/>
  <img src="https://github.com/k660323/ShotShot/blob/main/Images/%EB%AA%A8%EB%93%9C%20%EC%84%A0%ED%83%9D.JPG" width="49%" height="300"/>
  <img src="https://github.com/k660323/ShotShot/blob/main/Images/%EB%A7%B5%20%EB%A1%9C%EB%93%9C.JPG" width="49%" height="300"/>
  <img src="https://github.com/k660323/ShotShot/blob/main/Images/%EC%BA%90%EB%A6%AD%ED%84%B0%20%EC%84%A0%ED%83%9D.JPG" width="99%" height="600"/>
  <img src="https://github.com/k660323/ShotShot/blob/main/Images/%EC%A0%84%ED%88%AC%EC%94%AC.JPG" width="99%" height="600"/>
  
  < 게임 플레이 사진 >
</div>

+ BoardGame이란?
  + 탑 뷰 2D대전 액션 게임 입니다.
  + 다양한 캐릭터중 하나를 선택해 정해진 시간동안 적을 많이 사살하라
 
+ 목표
  + 정해진 시간 동안 적을 많이 섬멸시키는 플레이어 또는 팀이 승리합니다.

+ 게임 흐름
  + 매칭시 자동으로 세션이 생성되고 플레이어를 기다립니다. (최초 세션 생성시 그 플레이어가 세션권한을 갖습니다.)
  + 세션권한을 가진 플레이어가 게임을 시작하면 모드를 선택 후 게임 씬을 로드합니다.
  + 게임 플레이할 월드는 랜덤으로 로드됩니다.
  + 게임씬 입장후 플레이할 캐릭터를 선택합니다.
  + 캐릭터 선택후 제한 시간안에 적을 많이 처치하거나 목표 킬수를 달성하시면 승리입니다.

<br>

## 2. 프로젝트 정보

+ 사용 엔진 : UNITY
  
+ 엔진 버전 : 2020.3.19f1 LTS

+ 사용 언어 : C#
  
+ 작업 인원 : 1명
  
+ 작업 영역 : 콘텐츠 제작, 디자인, 기획
  
+ 장르      : 액션 게임
  
+ 소개      : Photon 에셋을 활용하여 만든 멀티플레이 2D 액션 게임이다.
  
+ 플랫폼    : PC
  
+ 개발기간  : 2021.10.06 ~ 2022.05.26
  
+ 형상관리  : GitHub Desktop

<br>

## 3. 캐릭터

### 캐릭터 ###
+ 총 5가지의 캐릭터가 구현되어 있습니다.
  
**톰맨 (라이플)**
| 스킬 이름 | 종류 | 설명 |
|:---:|:---|:---|
| 광전사 | 패시브 | 보유 체력이 적을 수록 기본 공격력 증가 |
| 덤블링 | 스킬 | 빠르게 이동하는 이동스킬 |              
| 폭탄 | 스킬 | 목표 지점 도착후 1초뒤에 터지는 폭탄|
| 자가 회복 | 스킬 | 2초뒤 체력을 소량 회복한다|
| 전투 자극제 | 궁극기 | 10초간 공격속도와 이동속도가 50%상승합니다. |
                    
**샷퍼 (샷건)**
| 스킬 이름 | 종류 | 설명 |
|:---:|:---|:---|
| 견고한 방패 | 패시브 | 기본체력이 +50 상승합니다. |
| 덤블링 | 스킬 | 빠르게 이동하는 이동스킬 |              
| 차징샷/확장 탄창 | 패시브 | 기본공격이 차징이 됩니다 / 추가 탄창 증가 |
| 재빠른 몸놀림 | 패시브 | 적을 처치시 덤블링 쿨이 초기화 / 장전속도 장전중 이동속도 증가 |
| 지뢰 | 궁극기 | 밝으면 터지는 지뢰를 설치합니다. |

**스니프 (스나이퍼)**
| 스킬 이름 | 종류 | 설명 |
|:---:|:---|:---|
| 은폐 | 패시브 | 3초간 비전투시 은폐 |
| 덤블링 | 스킬 | 빠르게 이동하는 이동스킬 |              
| 거미지뢰 | 스킬 |  주변에 적이 있으면 폭팔하는 거미지뢰 |
| 테이저건 | 스킬 | 적을 3초간 기절 시킵니다. |
| 닐스나이핑 | 궁극기 | 닐 스나이퍼에게 지원요청을 하여 적에게 대미지를 가하고 기절 시킨다 |   
                
**레인저 (리볼버)**
| 스킬 이름 | 종류 | 설명 |
|:---:|:---|:---|
| 날렵함 | 패시브 | 이동속도 증가 |
| 덤블링 | 스킬 | 빠르게 이동하는 이동스킬 |              
| 헤드샷 | 스킬 |  머리를 노려 강력한 데미지를 넣는다. |
| 전방 사격 | 스킬 | 전방에 무자비 사격을 가한다. |
| 난사 | 궁극기 | 사방에 총알을 난사한다. |   

**애쉬 (활)**
| 스킬 이름 | 종류 | 설명 |
|:---:|:---|:---|
| 크리티컬 | 패시브 | 30% 확률로 기본 공격시 데미지 2배 |
| 덤블링 | 스킬 | 빠르게 이동하는 이동스킬 |              
| 멀티샷 | 스킬 |  전방으로 뻗어가는 화살을 발사합니다. |
| 천공의 화살 | 스킬 | 하늘에 무수한 화살을싸서 해당지점에 퍼붓습니다. |
| 폭풍 화살 | 궁극기 | 마우스 커서 위치로 일정 시간 무자비하게 화살을 발사합니다. |   

<br>

<br>

---

<br>

## 4. 구현에 어려웠던 점과 해결과정
+ Photon에서 생성한 오브젝트를 오브젝트 풀링으로 구현하는데 어려웠습니다.
  + 오브젝트 소유자만이 오브젝트 풀링을 사용하고 소유자가 아닐경우 오브젝트를 비활성화 하도록 구현하였습니다.
  
+ 당시에는 유니티 에셋 스토어에는 제작에 필요한 적절한 2d에셋이 거의 존재하지 않아서 외부에서 들고와서 따로 수정 해야하는 불편함이 있었습니다.
  + 
    
+ 유저 목록, 정보를 정렬하여 구현하는데 많은 시간이 걸렸습니다.
  + 추후 다른 프로젝트에서 정렬 구현은 STL을 이용하여 빠르고 손쉽게 구현하도록 하였습니다.

+ 데이터를 모두 수작업으로 유니티 인스펙터에서 설정하는게 힘들었습니다.
  + 추후 다른 포트폴리오에서 데이터를 Json으로 만든 다음 역 직렬화를 해서 데이터를 불러오도록 하여 수작업 하던 것 들을 간편하게 불러오고 수정하도록 구현하였습니다.

## 5. 느낀점
+ 상태머신의 필요성을 느낌 모든 상태를 Boolean으로 관리하니 코드가 많아질수록 예상치 못한 작동 발생하여 버그를 수정하는데 많은 시간이 소요되었습니다.
+ 중복된 코드가 많아 작업이 오래걸리고 코드가 지저분하게 많아져 적극적인 상속 구조 설계가 필요함을 느낌
+ 핵심 객체를 총괄하는 싱글톤 매니저의 필요성 느낌
+ 데이터 파일을 생성하여 중복된 코드를 제거하고 접근의 용의함을 느낌 

## 6. 플레이 영상
+ https://www.youtube.com/watch?v=Uw-RA8RKhhw
