# [Unity 3D] ShotShot
## 1. 소개

<div align="center">
  <img src="https://github.com/k660323/FunnyLand/blob/main/Images/%EC%B5%9C%ED%9B%84%EC%9D%98%20%EC%83%9D%EC%A1%B4%EC%9E%90.JPG" width="49%" height="230"/>
  <img src="https://github.com/k660323/FunnyLand/blob/main/Images/%EC%A2%80%EB%B9%84%20%EC%84%9C%EB%B0%94%EC%9D%B4%EB%B2%8C.JPG" width="49%" height="230"/>
  <img src="https://github.com/k660323/FunnyLand/blob/main/Images/%EC%8A%88%ED%8C%85%EC%8A%88%ED%84%B0.JPG" width="49%" height="230"/>
  <img src="https://github.com/k660323/FunnyLand/blob/main/Images/%EB%A6%BF%EC%A7%80%20%EB%B8%94%EB%A1%9D%EC%BB%A4.JPG" width="49%" height="230"/>
  
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
+ P2P 네트워크 구조에 대한 이해가 부족해서 동기화가 잘 되지 않았습니다.
  + Photon 공식 홈페이지나 Youtube 영상을 참고하거나 관련 있는 지식인 분에게 질문을 해서 문제 해결 및 구현하였습니다.
  
+ Solid원칙을 지키지 않아 코드가 복잡하고 이해하기 어렵고 확장 및 설계에 하는데 많은 어려움이 있었고 개발 시간이 많이 지연되었습니다.
  + 추후 정보처리기사 자격증을 준비하면서 Solid원칙을 이해하고 추후 포트폴리오에서 이 원칙을 지키도록 노력하였습니다.
    
+ 데이터를 모두 수작업으로 유니티 인스펙터 창 에서 설정 하는 게 힘들었습니다. 
  + 추후 다른 포트폴리오에서 데이터를 Json으로 만든 다음 역 직렬화를 해서 데이터를 불러오도록 하여 수작업 하던 것 들을 간편하게 불러오고 수정하도록 구현하였습니다.
 
## 5. 느낀점
+ Zombie Suriver보다는 코드가 정리되고 간결화 되었지만 아직 부족하다 객체간 접근이 불편하고 너무 나눠져 있어서 불편하다. 다음 프로젝트에는 매니저를 둬서 명확하게 관리의 필요성을 느낌
+ 매번 프로젝트할때 처음부터 만들면 시간이 많이 걸리고 코드 편의성도 많이 떨어져 템플릿 프로젝트가 필요함을 느낌
+ Photon Asset와 네트워크 프로그래밍 이해도가 낮다고 느낌 좀 더 문서와 네트워크 공부가 필요함

## 6. 플레이 영상
+ https://www.youtube.com/watch?v=VdKAyiOhw1c
