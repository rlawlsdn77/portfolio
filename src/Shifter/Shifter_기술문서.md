# SHIFTER

> Shift 키 하나로 캐릭터의 색을 흰색 ↔ 검은색으로 바꾸면, 밟을 수 있는 발판과 맞는 총알이 한꺼번에 뒤집히는 2D 퍼즐 플랫포머
>
> Unity 6 · C# · 1인 개인 프로젝트 · 2026.06.24 \~ 2026.07.10

![흰색 상태로 흰 발판 사이를 뛰어오르는 캐릭터. 오른쪽의 검은 발판은 반투명해 지금은 밟을 수 없다](images/01_cover.png)

---

## 1. 게임 소개

SHIFTER는 캐릭터의 색을 흰색과 검은색으로 바꿔 가며 스테이지를 통과하는 2D 플랫포머입니다. 규칙은 하나, **캐릭터와 같은 색인 것만 실제로 존재합니다.** 흰색일 때는 흰 발판만 밟히고 흰 총알에만 맞습니다. 그래서 Shift 키를 누르는 순간 '밟을 수 있는 길'과 '피해야 할 총알'이 함께 뒤집힙니다. 스테이지 2개와 보스전 1개로 이루어져 있습니다.

이동과 공격만 있는 게임에서는 키를 눌러도 캐릭터 하나가 움직일 뿐이라, 입력에 비해 돌아오는 변화의 크기가 작다고 판단했습니다. 그래서 키 하나가 발판·총알·퍼즐을 동시에 바꾸는 구조를 택했습니다.

목표한 경험은 **"색을 고르는 키 한 번으로 그 색의 길과 위험을 함께 떠안아, 작은 입력이 눈앞의 판 전체를 바꾸는 경험"** 입니다.

| 조작 | 키 | 결과 |
|---|---|---|
| 이동 · 점프 | A · D(← · →) · Space | 좌우 이동, 공중에서 한 번 더 뛰는 2단 점프 |
| 색 바꾸기 | Shift | 처음엔 회색, 이후 흰색 ↔ 검은색. 한 번 쓰면 1초 뒤에 다시 쓸 수 있음 |
| 일시정지 | Esc | 일시정지 · 해제 |
| 다음 스테이지 | 포탈 안에서 W(↑) | 다음 씬으로 이동 |
| 레이저 조준 | 마우스 | 레이저 퍼즐에서 빛을 쏠 방향 |

**참고한 게임.** 빛·어둠 두 색을 바꿔 가며 같은 공간을 다르게 통과하는 2D 플랫포머 *Outland*를 참고했습니다. Outland에서는 내 색과 같은 총알이 무해하지만, SHIFTER는 반대로 같은 색 총알에 맞습니다. 같은 색이어야 밟히는 발판과 맞물려, 한 색을 고르면 그 색의 발판과 총알을 함께 떠안게 했습니다. 색을 바꾸는 데에는 1초 대기 시간(쿨타임)을 두어, 연타로 모든 상황을 넘기지 못하게 했습니다.

![참고한 게임 Outland의 플레이 화면(Steam 상점 스크린샷). 빨간색 캐릭터 왼쪽에 파란 탄막, 오른쪽에 빨간 탄막이 있다](images/outland.jpg)

| 항목 | 내용 |
|---|---|
| 엔진 · 언어 | Unity 6 (6000.3.15f1), URP 2D · C# |
| 사용한 패키지 | Input System(키 입력), uGUI + TextMeshPro(화면 UI) |
| 외부 에셋 | 캐릭터·보스·타일·배경·UI 그림과 Super Pixel Projectiles Pack 1(untiedgames, 보스 총알 그림). 모두 구매 또는 무료 에셋 |
| 레벨 제작 | 발판 틀(프리팹)을 씬에 직접 배치 (맵 편집 도구 LDtk는 초기에 쓰다가 맵 파일을 지움) |
| 도구 | JetBrains Rider, Git · GitHub, Claude(→ [9장 AI와 함께 만든 방식](#ai와-함께-만든-방식)) |
| 규모 | 직접 쓴 C# 스크립트 33개(약 1,900줄, 배운 코드 제외), 씬 4개 |

- GitHub: https://github.com/rlawlsdn77/Unity-2D-Shifter

---

## 2. 한 판의 흐름

메뉴에서 시작해 STAGE 1 → STAGE 2 → 보스전을 차례로 지나면 한 판이 끝납니다. 각 스테이지 끝의 포탈에서 W를 누르면 다음 씬으로 넘어가고, 보스를 쓰러뜨린 뒤의 포탈은 메뉴로 돌아갑니다. 스테이지마다 HP 100, 회색에서 새로 시작합니다.

| STAGE 1 | STAGE 2 | STAGE BOSS |
|---|---|---|
| ![STAGE 1 전구 퍼즐](images/08_stage1_lights.png) | ![STAGE 2 레이저 조준](images/09_stage2_laser.png) | ![보스전 운석 공격](images/11_boss_eruption.png) |
| 레드 타일, 흰·검 총알 발사기, 박스 운반, 전구 퍼즐 | 움직이는 발판, 거울로 꺾는 레이저 퍼즐, 밀려오는 흰·검 벽(크러셔 벽) | 보스 AGIS - RED MARTYR. 공격 3번 뒤 5초 동안 드러나는 약점을 위에서 밟아 3번 맞히면 격파 |

![STAGE 2, 흰색 상태. 왼쪽 위에 지금 색, 아래에 HP 바가 있습니다. 캐릭터가 선 회색 돌 모양이 흰 발판이고, 오른쪽에 반투명하게 보이는 어두운 발판은 지금 색으로는 밟을 수 없는 검은 발판입니다](images/02_ingame.png)

플레이어가 되풀이하는 행동은 아래 고리입니다.

```mermaid
flowchart LR
    A["앞의 발판 · 총알<br/>색 읽기"] --> B{"지금 색으로<br/>지나갈 수 있나?"}
    B -->|"예"| C["이동 · 점프"]
    B -->|"아니요"| D["Shift<br/>(쿨타임 1초)"]
    D --> C
    C --> E["퍼즐 해결<br/>→ 문 열림"]
    E --> A
```

이 고리에서 Shift는 '길을 여는 열쇠'이자 '새 위험을 부르는 선택'입니다. 흰 발판을 밟으려고 흰색이 되는 순간 흰 총알에 맞을 수 있게 되므로, 플레이어는 **바꿀지와 언제 바꿀지**를 계속 고릅니다. 쿨타임 1초 때문에 바꾼 결정을 바로 되돌릴 수도 없습니다. 키 한 번의 결과가 화면 전체에 미치게 하려는 목표한 경험이 이 반복에서 나옵니다.

---

## 3. 전체 구조

```mermaid
%%{init: {"layout": "elk"}}%%
flowchart TB
    IN["키 입력 · SimpleInput"]
    PC["ShifterController<br/>이동 · 점프 · Shift · 일시정지"]
    CC["CharacterController2D<br/>막힌 곳을 재며 이동"]
    SH["Shifter<br/>HP · 지금 색"]
    PM["PlatformManager<br/>발판 밟힘 · 투명도"]
    HZ["위험 요소<br/>Red · Bullet · BossProjectile"]
    PZ["기믹<br/>CrusherWall · 레이저 퍼즐 · 박스"]
    BS["보스<br/>BossManager → 공격 3종"]

    IN --> PC
    PC -->|"SetMoveInput · RequestJump"| CC
    PC -->|"ShiftTag()"| SH
    PM -.->|"캐릭터 색 읽기"| SH
    HZ -->|"TakeDamage"| SH
    HZ -.->|"캐릭터 색 읽기"| SH
    PZ -->|"AddExternalMovement"| CC
    BS -->|"총알 생성 · 정리"| HZ
```

*실선은 직접 부르는 관계, 점선은 값을 읽기만 하는 관계입니다.*

코드는 아래 세 가지 원칙으로 나눴습니다. 기능이 늘어도 '색'과 '이동'이 엇갈리지 않게 하려는 것이었습니다.

1. **캐릭터의 색을 바꾸는 곳은 `Shifter.ShiftTag()` 하나.** 발판·총알·벽은 모두 캐릭터의 색을 읽기만 합니다. 바꾸는 곳이 하나라 서로 다른 색을 보는 일이 없습니다. → [4장](#4-색-규칙-shift-한-번이-바꾸는-것)
2. **캐릭터 이동은 `CharacterController2D`만 맡는다.** 키 입력도, 벽이 미는 힘도 이 코드를 거치며, 움직이기 전에 앞이 막혔는지 먼저 잽니다. → [5장](#5-거꾸로-쓰는-색-규칙-크러셔-벽), [7장](#7-캐릭터-이동과-카메라)
3. **보스 공격은 자기가 만든 총알을 스스로 치운다.** 보스 관리자는 공격에게 '시작'과 '정리'만 요청합니다. → [6장](#6-보스전)

| 클래스 | 맡는 일 |
|---|---|
| `ShifterController` | 키 입력 → 이동·점프 요청, Shift 쿨타임, 일시정지 |
| `Shifter` | HP와 HP 바, 색 바꾸기, 맞으면 깜빡임, 게임오버 |
| `CharacterController2D` (배운 코드 + 박스 밀기) | 막힌 곳을 재며 이동, 중력·2단 점프, 밖에서 미는 힘 |
| `PlatformManager` | 흰·검 발판의 밟힘과 투명도를 캐릭터 색에 맞춤 |
| `CrusherWall` · `BoxGoal` · `LightBeam` 등 | 스테이지 기믹 |
| `BossManager` · `IBossPattern` 구현 3종 | 공격 교체 · 그로기 · 사망 / 공격 하나의 실행과 정리 |

---

## 4. 색 규칙: Shift 한 번이 바꾸는 것

목표한 경험의 중심입니다. 키 한 번이 발판·총알·퍼즐을 함께 바꾸려면, 이 셋이 **같은 규칙 하나**를 따라야 했습니다.

### 4.1 같은 색이면 실체, 다른 색이면 허상

![STAGE 1 시작. 회색에서 Shift로 흰색이 되어 흰 발판에 올라서고, 검은색이 되면 그 발판이 꺼져 아래로 빠집니다. 검은 발판에 올라선 뒤 다시 흰색이 되면 또 빠집니다](images/shift.gif)

| 대상 | 캐릭터와 같은 색 | 다른 색 |
|---|---|---|
| 흰·검 발판 | 밟힘 · 불투명 | 몸이 빠져나감 · 반투명(50%) |
| 회색·빨강 발판 | 항상 밟힘 | 항상 밟힘 (빨강은 닿아 있는 동안 HP가 깎임 — STAGE 2는 지금 버그 → [9장](#남은-문제)) |
| 총알 (STAGE 1 · 보스) | 맞음 (STAGE 1은 지금 버그 → [9장](#남은-문제)) | 몸을 지나감 |
| 크러셔 벽 (STAGE 2) | 지나감 | 밀려남 — 일부러 뒤집은 규칙 (→ [5장](#5-거꾸로-쓰는-색-규칙-크러셔-벽)) |

캐릭터는 회색으로 시작합니다. 회색은 흰·검 발판을 밟을 수 없지만 흰·검 총알에도 맞지 않는, '안전하지만 멀리 갈 수 없는' 상태입니다. 첫 Shift는 항상 흰색이고, 이후로는 흰색 ↔ 검은색만 오가며 회색으로 돌아가는 길은 없습니다.

회색으로 시작하게 한 것은 **Shift를 쓰게 하려는 것**입니다. 회색은 흰·검 어느 쪽과도 맞지 않아, 흰·검 발판은 밟을 수 없어 빠지고 크러셔 벽에는 모두 막혀 끌려갑니다. 한 번 Shift한 뒤 회색으로 돌아갈 수 없게 한 것은, 회색이 어떤 총알에도 맞지 않기 때문입니다. 돌아갈 수 있으면 무적 상태로 보스전을 진행할 수 있습니다.

### 4.2 다른 색 발판은 아예 없는 것이 된다

처음부터 태그로 만들었습니다. 캐릭터·발판·총알이 같은 글자 하나로 비교되는 단순함이 "같은 색인가"라는 게임 규칙과 그대로 맞았습니다. 대가로 캐릭터가 'Player' 태그를 달 수 없어 오브젝트 이름 `"Shifter"`로 캐릭터를 알아보게 됐습니다(6개 파일 7곳). 이 대가는 [9장](#9-돌아보며)의 회고로 이어집니다.

**다른 색 발판을 어떻게 '없는 것'으로 만들까?**

| | 발판의 콜라이더를 끔 | 캐릭터 쪽에서 '무엇에 부딪힐지'만 바꿈 |
|---|---|---|
| 바꾸는 것 | 발판 수만큼 켜고 끔 | 캐릭터 설정 값 하나(`collisionLayer`) |
| 미치는 범위 | 캐릭터 이동, 레이저 등 콜라이더를 보는 모든 기능 | 캐릭터 이동뿐 |
| 대가 | 흰·검 태그지만 발판이 아닌 것(총알, 발판 프리팹을 다시 쓴 크러셔 벽)은 관리 대상에서 빼야 함 | 레이저처럼 새 기능마다 같은 규칙을 다시 만들어야 함 |

발판의 콜라이더를 끄기로 했습니다. 첫 구현 때 두 방법을 견줘, 나중에 만들 기능도 콜라이더를 보기만 하면 같은 색 규칙을 따르도록 이쪽을 골랐습니다. 그래서 2주 뒤 만든 레이저 퍼즐이 **추가 코드 없이** 색 규칙을 따랐습니다. 조준 중에 Shift로 빛의 길을 열고 닫는 기믹이 여기서 생겼습니다. 같은 이유로 흰색 직선 풀이도 생겼습니다.

### 4.3 색을 바꾸는 곳은 하나, 발판은 읽기만

Shift가 바꾸는 것은 캐릭터의 색 하나뿐이고, 발판·총알·벽·화면 표시는 각자 필요한 때에 그 색을 읽기만 합니다.

```mermaid
flowchart LR
    K["Shift 키<br/>쿨타임 1초"] --> T["캐릭터 색 바꾸기"]
    T -.->|"매 프레임"| PM["발판<br/>밟힘 · 투명도"]
    T -.->|"닿는 순간"| PR["총알<br/>같은 색이면 맞음"]
    T -.->|"물리 계산마다"| CW["크러셔 벽<br/>다른 색이면 밀어냄"]
    T -.->|"바뀐 순간만"| UI["화면 색 표시"]
```

색은 Unity의 태그로 나타냅니다. 캐릭터·발판·총알이 모두 `Common`·`White`·`Black` 중 하나를 달고, 판단은 "태그가 같은가"(`CompareTag`) 하나로 합니다.

```csharp
// Shifter.cs — 흰색이면 검은색, 그 밖(회색 포함)은 흰색
public void ShiftTag()
{
    gameObject.tag = (gameObject.CompareTag(TAG_WHITE)) ? TAG_BLACK : TAG_WHITE;
    currentTag = gameObject.tag;
}
```

발판마다 캐릭터와 태그를 비교해 콜라이더를 켜고 끕니다.

```csharp
// PlatformManager.cs
private void ColliderControll()
{
    foreach (BoxCollider2D col in allColliders)
    {
        GameObject obj = col.gameObject;

        if (obj.CompareTag(TAG_COMMON) || obj.CompareTag(TAG_RED))
        {
            col.enabled = true;           // 무색: 항상 단단하다
        }
        else if (obj.CompareTag(player.tag))
        {
            col.enabled = true;           // 같은 색: 단단하다
        }
        else
        {
            col.enabled = false;          // 다른 색: 없는 것과 같다
        }
    }
}
```

- 새 발판은 흰·검 발판 프리팹을 놓기만 하면 됩니다. `PlatformManager`가 태그로 발판을 찾아 관리하므로, STAGE 2의 발판 20개(흰 9 · 검 11)를 코드 수정 없이 넣었습니다.

### 4.4 꺼진 발판은 빛도 지나간다

STAGE 2의 레이저 퍼즐은 버튼을 밟으면 조준 모드가 되고, 마우스로 빛을 쏴 거울 3개에서 꺾어 센서에 맞히면 문이 열립니다. 조준하는 동안 캐릭터는 움직일 수 없지만 Shift는 쓸 수 있습니다. 제자리에서 색을 바꿔 빛이 지나갈 발판을 고르게 하려는 것입니다.

빛 알갱이를 날려 물리로 튕기게 하면 같은 각도로 쏴도 경로가 달라질 수 있어, 레이캐스트로 경로를 이어 붙였습니다. 센서에 닿았는지도 경로의 끝점만 보면 됩니다.

레이캐스트는 꺼진 콜라이더를 보지 못하므로, 다른 색 발판은 빛에게도 '없는 것'이 됩니다. 아래 빛 계산(`LightBeam.FireBeam`, 매 프레임)의 흐름에는 색을 확인하는 갈림길이 하나도 없습니다.

```mermaid
flowchart LR
    S["조준 방향으로<br/>레이캐스트"] --> W{"무엇에 맞았나?"}
    W -->|"거울"| R["반사해서<br/>다시 쏨"]
    R --> S
    W -->|"센서"| OK(["문 열림"])
    W -->|"벽 · 켜진 발판"| E(["멈춤"])
```

*그림에서 뺀 세부 — 아무것도 맞지 않으면 사거리 끝까지 선을 긋고 끝나고, 캐릭터 몸·버튼 같은 트리거는 그대로 통과하고, 같은 면을 다시 맞히지 않게 맞은 곳에서 0.05 앞으로 옮겨 다시 쏘며, 반사 6회·반복 11회에서 멈춥니다. 센서에 닿았는지는 프레임마다 다시 계산해 센서(`LightSensor`)에 알립니다.*

| 검은색으로 수평 조준 | 흰색으로 같은 수평 조준 |
|---|---|
| ![검은색이면 수평 빛이 검은 발판에 막힌다](images/laser_black_blocked.png) | ![흰색이면 검은 발판이 꺼져 빛이 센서까지 곧게 간다](images/laser_white_straight.png) |

오른쪽 직선 풀이는 의도하지 않은 배치 실수입니다. 의도한 풀이는 검은색으로 위쪽을 겨눠 거울 3개를 거치는 길이었습니다. 그런데 흰색이 되면 다른 색인 검은 발판이 콜라이더째 꺼지고, 빛이 그 자리를 그대로 지나갑니다. 모든 기능이 같은 규칙을 따르게 만들었기 때문에, 레벨을 만들 때 **그 규칙이 열어 주는 길까지** 확인했어야 했습니다. 아직 고치지 않은 문제로 [9장](#9-돌아보며)에 적었습니다.

![의도한 풀이. 검은색으로 수평 조준하면 검은 발판에 막히고, 위로 옮기면 거울 3개를 거쳐 오른쪽 센서에 닿습니다](images/laser.gif)

레이저 퍼즐에서 따로 막아 둔 경우는 하나입니다.

| 상황 | 처리 |
|---|---|
| 이미 풀린 레이저 퍼즐의 버튼을 다시 밟음 | 무시. 한 번 풀리면 빛이 벗어나도 문은 열려 있음 |

---

## 5. 거꾸로 쓰는 색 규칙: 크러셔 벽

크러셔 벽은 발판에서 익힌 색 규칙을 거꾸로 써서, **Shift를 누르는 판단과 타이밍**을 다시 과제로 만든 STAGE 2의 마지막 구간입니다. 벽이 캐릭터를 미는 동작은, 엔진의 자동 물리를 쓰지 않는 이 게임의 캐릭터(→ [7장](#7-캐릭터-이동과-카메라))에서는 직접 만들어야 했습니다.

### 5.1 같은 색이면 지나가고, 다르면 끌려간다

STAGE 2는 처음 흰·검 총알 개수를 세는 퍼즐로 만들고 있었습니다. 다음 날 이것을 지금의 레이저 퍼즐과 크러셔 벽으로 바꿨습니다.

| | 흰·검 총알 개수 세기 | 레이저 퍼즐 + 크러셔 벽 |
|---|---|---|
| 플레이어가 하는 일 | 지나가는 총알을 보고 개수를 셈 | 빛의 길을 고르고, 밀려오는 벽마다 색을 맞춤 |
| Shift의 역할 | 거의 없음 | 조준 중 길 바꾸기, 벽마다 통과할 색 고르기 |

Shift를 쓸 이유가 약해서 바꿨습니다. 개수 세기는 관찰과 기억의 퍼즐이라, 이 게임의 중심인 '색을 바꾸는 판단'이 끼어들 자리가 없었습니다.

크러셔 벽은 1.2초마다 흰색 또는 검은색 벽이 초당 10유닛으로 밀려오는 구간입니다. 발판과 규칙을 반대로 했습니다. 벽과 **같은 색이면 몸이 지나가고, 다른 색이면 벽에 걸려 끝 지점까지 끌려갑니다.** 발판에서 익힌 "같은 색 = 단단함"을 마지막 구간에서 거꾸로 쓰게 하는 변주입니다. 바닥은 레드 타일입니다.

벽도 발판처럼 캐릭터와 다른 색이면 반투명하게 보입니다. 반투명은 '내 색과 다르다'는 표시라, 발판에서는 '지나갈 수 있다', 벽에서는 '막힌다'는 뜻이 됩니다.

![흰 벽에 밀리다가 Shift로 흰색이 되어 통과하고, 다음 검은 벽도 색을 맞춰 통과하는 장면](images/crusher.gif)

| | 코드 기본값 | 플레이해 보며 맞춘 값 |
|---|---|---|
| 벽 속도 | 초당 3 | 초당 10 (캐릭터 초당 8보다 빠름) |
| 벽 간격 | 0.7초 | 1.2초 |
| Shift 쿨타임 | 2초 | 1초 |

처음 코드 기본값에서는 벽이 느려 피해 달아날 수 있었습니다. Shift를 적극적으로 써서 기믹을 풀게 하려고, 벽을 캐릭터보다 빠르게 바꿔 도망칠 수 없게 했습니다. 간격과 쿨타임도 플레이해 보며 맞춘 값이라, 쿨타임이 벽 간격보다 짧아 벽마다 색을 맞출 수는 있습니다.

### 5.2 벽에 걸린 캐릭터는 벽이 직접 민다

**크러셔 벽과 캐릭터의 충돌을 어떻게 처리할까?** 실제로 해 본 순서대로 적습니다.

| | 벽을 Dynamic Rigidbody로 두고 물리로 밀기 | 벽 콜라이더를 켜고 이동 코드에 맡김 | 벽 콜라이더를 끄고 겹침을 직접 확인해 밀기 |
|---|---|---|---|
| 추가 코드 | 적음 | 없음 | 겹침 확인 + 미는 양 계산 |
| 문제 | 캐릭터는 물리 힘을 받지 않아(키네마틱) 결국 위치를 직접 옮겨야 함 | 겹치는 순간 캐릭터가 얼어붙음 | 그림 사각형으로 겹침을 보므로 단순한 모양에만 맞음. `Rigidbody2D` 우회가 필요 |

두 번째 시도에서는 벽이 움직여 캐릭터와 겹치는 순간, 캐릭터가 벽 속에서 꼼짝하지 못했습니다.

원인을 찾을 때는 먼저 Shift로 콜라이더가 켜지고 꺼지는 것과 관련이 있는지 AI에게 물었고, AI가 원인을 짚었습니다. 캐릭터는 움직이기 전에 '가려는 방향으로 상자를 밀어 보아' 부딪히는지 잽니다(→ 7.1의 `ResolveAxisMovement`). 이미 겹쳐 있으면 어느 방향으로 재도 '바로 앞이 막힘'(거리 0)이 나와, 움직일 수 있는 거리가 늘 0이 됩니다. 이 이동 코드에는 겹침을 풀어 주는 처리가 없습니다. 로그를 찍어 보니 벽에 있는 동안 계속 같은 값이 나와, 이 원인이 맞다는 것을 확인했습니다.

세 번째를 골랐습니다. 이동 판정을 `CharacterController2D`가 쥐고 있는 구조에서는 **캐릭터 이동을 그 코드 하나에 모으는 것**이 가장 안전하다고 판단했습니다. 이동 코드가 하나면 어떤 힘이 더해져도 같은 막힘 검사를 거칩니다.

플레이로 확인했습니다. 다른 색 벽에 걸리면 벽과 함께 끝 지점까지 밀려가고, 그 사이 Shift로 같은 색이 되면 그 자리에서 벽이 몸을 지나갑니다(5.1 GIF).

### 5.3 벽이 움직인 만큼 민다

벽이 움직일 때마다 캐릭터와 색을 비교해, 다른 색이면 벽이 움직인 만큼 캐릭터를 밀어 줍니다.

```csharp
// CrusherWall.cs (발췌) — 물리 계산마다, 벽 하나씩
bool blocked = !player.CompareTag(wall.obj.tag);     // 다른 색이면 막힌다 (발판과 반대)
color.a = blocked ? 0.5f : 1f;                       // 다른 색 벽은 반투명 (5.1)

// ... 벽을 끝 지점 쪽으로 옮기고, 이번에 움직인 거리를 wallDelta에 담는다

if (blocked && wall.renderer.bounds.Intersects(playerCollider.bounds))
{
    playerController.SetMoveInput(0f);
    playerController.AddExternalMovement(wallDelta);  // 벽이 움직인 만큼 캐릭터도 움직인다
}

// SpawnWall() — 벽을 만들 때
col.enabled = false;                                 // 켜 두면 캐릭터가 벽 속에서 얼어붙는다 (5.2)
Rigidbody2D rb = obj.AddComponent<Rigidbody2D>();    // PlatformManager가 이 벽을 발판으로 관리하지 않게
rb.bodyType = RigidbodyType2D.Kinematic;             // 중력·물리 힘을 받지 않게 (벽은 직접 옮김)
```

- **겹침은 그림의 사각형으로 직접 확인합니다.** 콜라이더를 끈 대신 벽 그림과 캐릭터가 겹치는지 봅니다.
- **미는 힘도 이동 코드를 거칩니다.** `AddExternalMovement`는 미는 거리를 더해 둘 뿐이고, 실제 이동은 `CharacterController2D`가 앞이 막혔는지 잰 뒤에 합니다(→ 7.1). 그래서 밀려난 캐릭터가 다른 지형을 뚫지 않습니다.
- **`Rigidbody2D`를 붙이는 이유.** 벽은 흰·검 발판 프리팹을 세워 다시 씁니다. 그대로 두면 `PlatformManager`가 태그를 보고 콜라이더를 다시 켜므로, '`Rigidbody2D`가 있으면 건너뛴다'는 규칙에 걸리게 했습니다.

벽이 캐릭터를 미는 차례는 아래와 같습니다. 그림에서 볼 것은, 벽은 미는 거리를 넘겨 줄 뿐이고 실제로 캐릭터를 옮기는 것은 이동 코드(`CharacterController2D`)라는 점입니다.

```mermaid
sequenceDiagram
    participant W as CrusherWall
    participant C as CharacterController2D
    participant P as Physics2D
    Note over W: 물리 계산마다, 벽 하나씩
    W->>W: 벽을 끝 지점 쪽으로 옮기고 wallDelta에 담음
    alt 캐릭터와 다른 색이고 겹침
        W->>C: SetMoveInput(0)
        W->>C: AddExternalMovement(wallDelta)
    end
    Note over C: FixedUpdate
    C->>C: desireMove = 입력 + externalDelta
    C->>P: BoxCast (ResolveAxisMovement, 가로 · 세로)
    P-->>C: 막힌 거리
    C->>C: rBody.MovePosition(막힌 거리까지만)
```

---

## 6. 보스전

보스전에서는 쏟아지는 흰·검 총알 속에서 '어느 색이 될지'를 계속 고르다가, 보스가 멈춘 5초 안에 약점을 밟습니다. 색 판단을 가장 빠르게 반복하는 구간입니다. 스테이지에서 익힌 Shift 활용법으로, 탄막을 알맞은 타이밍에 Shift로 피한 뒤 약점을 공격하게 했습니다.

### 6.1 공격 3번 → 5초 빈틈 → 약점

```mermaid
stateDiagram-v2
    state "대기 (10초)" as Wait
    state "공격 3회 (각 10초, 무작위)" as Pattern
    state "그로기 (5초, 약점 열림)" as Groggy
    state "사망 연출 (2.1초)" as Dead
    [*] --> Wait: 보스방 입장
    Wait --> Pattern
    Pattern --> Groggy: 3회 공격 후
    Groggy --> Wait: 5초 경과
    Groggy --> Dead: 약점 3번째 성공 · 출구 열림
    Dead --> [*]
```

보스는 10초마다 공격을 무작위로 바꾸고, 3번 공격한 뒤 5초 동안 멈춰 약점을 드러냅니다(그로기). 약점은 밟는 즉시 꺼져 그로기 한 번에 한 번만 맞힐 수 있습니다. 한 주기가 45초(대기 10 + 공격 30 + 그로기 5)이므로, 가장 빠른 격파는 세 번째 그로기가 열리는 약 130초(2:10) 입니다(코드의 시간 값으로 계산).

| 공격 | 내용 | 피해 |
|---|---|---|
| 운석 (Eruption) | 보스 위 10유닛, 좌우 ±8 범위의 무작위 위치에서 1초마다 하나씩 떨어짐. 15개로 정했지만 10초 뒤 다음 공격으로 넘어가며 끊겨 실제로는 약 10개 | 10 |
| 난사 (Rampage) | 보스 중심에서 8방향으로 한꺼번에 쏘기를 1.5초 간격으로 5번 (40발) | 6 |
| 레이저 (Razer) | 캐릭터 방향 ±15° 안의 무작위 각도로 6\~12발, 0.7초 간격 | 8 |

- 총알마다 흰·검이 무작위로 정해지고, 같은 색일 때만 맞습니다. 흰·검 발판은 총알을 막지 않으므로, 발판 뒤에 숨는 대신 **다가오는 총알과 다른 색이 되어 몸으로 흘려보내는** 것이 회피 방법입니다.
- 개수·간격·속도는 보스 프리팹 기본값(운석 5개, 난사 3번, 레이저 5\~10발·속도 12 등)에서 플레이해 보며 난이도를 맞춘 값입니다.

| 운석 | 난사 | 레이저 |
|---|---|---|
| ![운석 공격](images/boss_eruption.gif) | ![난사 공격](images/boss_rampage.gif) | ![레이저 공격](images/boss_razer.gif) |

*공격 3종. 레이저는 보스 앞에서는 쏘자마자 사라지는 버그(→ [9장](#9-돌아보며)) 때문에, 보스에서 떨어진 자리에서 찍어 레이저가 화면을 가로지르는 모습입니다. 흰 총알은 그림의 원래 색 때문에 노란빛으로 보입니다.*

### 6.2 공격 3종을 인터페이스로 묶는다

**공격 3종의 공통 구조: 부모 클래스 vs 인터페이스**

| | 공통 부모 클래스(추상 클래스) `BossPattern` | 약속만 정하는 인터페이스 `IBossPattern` |
|---|---|---|
| 공유하는 것 | 동작(총알 목록, 정리 반복문)을 부모에 한 번만 | 약속(시작하면 끝낼 수 있어야 한다)만 |
| 이 게임에서 | 세 공격의 공통점은 시작·정리뿐이고 공격 방식(낙하, 원형 발사, 조준 발사)이 모두 달라 부모가 가질 동작이 거의 없음 | `BossManager`는 두 함수만 알면 되고, 공격은 방식을 자유롭게 구현 |
| 대가 | 가질 동작이 없어 빈 클래스가 됨 | '총알을 목록에 담고 정리 때 지우기'가 세 공격에 똑같이 반복됨 |

인터페이스로 정했습니다. 처음에는 빈 `BossPattern` 클래스로 시작했다가 지우고, 공격을 실제로 구현할 때 인터페이스로 바꿨습니다. 공격끼리 나눠 가질 것은 동작이 아니라 **약속**이라고 봤기 때문입니다. 정리 코드 반복은 3곳 × 8줄 정도라 감수했습니다. 다만 `BossManager`가 아직 공격의 실제 타입을 `switch`로 알고 있어, 새 공격을 넣으려면 `BossManager`도 고쳐야 합니다.

공격이 바뀔 때마다 보스 주변에 흰·검 발판을 무작위로 만드는 기능도 넣었다가 뺐습니다. 직접 플레이해 보니 흰·검 발판이 둘 다 캐릭터 자리에 생기면 어느 색으로 바꿔도 빠져나갈 수 없었고, 실력으로 피할 수 없는 실패가 무작위로 생기는 것은 공정하지 않다고 판단했습니다.

### 6.3 공격은 자기 총알을 스스로 치운다

```mermaid
classDiagram
    direction TB
    class OnBosspattern {
        보스방 입구 트리거
    }
    class BossManager {
        -IBossPattern currentPattern
        -int patternCount
        -bool isGroggy
        +BeginEncounter()
        +DamageToBoss()
        -ChangePattern()
        -StartGroggy()
        -EndGroggy()
        -BossDead()
    }
    class IBossPattern {
        <<interface>>
        +StartPattern()
        +EndPattern()
    }
    class BossPattern_Eruption {
        -List~GameObject~ meteorList
    }
    class BossPattern_Rampage {
        -List~GameObject~ activeProjectiles
    }
    class BossPattern_Razer {
        -List~GameObject~ activeLasers
    }

    OnBosspattern ..> BossManager : BeginEncounter()
    BossManager --> IBossPattern : currentPattern
    IBossPattern <|.. BossPattern_Eruption
    IBossPattern <|.. BossPattern_Rampage
    IBossPattern <|.. BossPattern_Razer
    BossPattern_Eruption ..> BossProjectile : 운석 생성 · 정리
    BossPattern_Rampage ..> BossProjectile : 총알 생성 · 정리
    BossPattern_Razer ..> BossProjectile : 레이저 생성 · 정리
```

*실선은 필드로 들고 있는 관계, 점선은 부르거나 만드는 관계입니다. `BossManager`는 공격을 `IBossPattern`으로 다루지만, 공격을 고를 때는 세 공격 클래스를 직접 들고 `switch`로 고릅니다(→ 6.2).*

약점과 총알 쪽은 이렇게 이어집니다. 총알(`BossProjectile`)은 캐릭터와 같은 색일 때만 피해를 줍니다.

```mermaid
classDiagram
    direction TB
    class BossProjectile {
        +Initialize(dir, spd, dmg, attributeTag)
        -OnTriggerEnter2D()
    }
    class BossHeart {
        -OnTriggerEnter2D()
    }
    class Boss {
        +int CurrentBossCount
        +int MaxBossCount
    }
    class Shifter {
        +float Hp
        +TakeDamage(damage)
    }

    BossManager --> BossHeart : 그로기 때만 켬
    BossHeart ..> Boss : CurrentBossCount--
    BossHeart ..> BossManager : DamageToBoss()
    BossManager --> Shifter : Hp +30
    BossProjectile ..> Shifter : 같은 색이면 TakeDamage
```

공격 하나는 '시작'과 '정리' 두 함수만 약속하는 인터페이스(`IBossPattern`)를 구현합니다.

```csharp
// IBossPattern.cs
public interface IBossPattern
{
    void StartPattern();
    void EndPattern();
}
```

난사 공격(`BossPattern_Rampage`)은 만든 총알을 목록에 담아 두고, 끝날 때 남은 것을 모두 지웁니다.

```csharp
// BossPattern_Rampage.cs (발췌)
public void StartPattern()
{
    activeProjectiles.Clear();
    StartCoroutine(RampageRoutine());
}

public void EndPattern()
{
    StopAllCoroutines();                            // 아직 쏘지 않은 발사도 취소
    foreach (var projectile in activeProjectiles)
    {
        if (projectile != null) Destroy(projectile);   // 이미 맞아서 사라진 총알은 null
    }
    activeProjectiles.Clear();
}
```

보스 관리자(`BossManager`)는 공격을 바꿀 때 이전 공격에게 정리만 맡기고, 세 번째 공격이 끝나면 그로기로 넘어갑니다.

```csharp
// BossManager.cs (발췌)
private void ChangePattern()
{
    if (currentPattern != null)
    {
        currentPattern.EndPattern();                // (1) 이전 공격은 스스로 정리
    }

    patternCount++;
    if (patternCount > 3)                           // (2) 3번 뒤 그로기
    {
        StartGroggy();
        return;
    }

    int randomPattern = Random.Range(0, 3);
    switch (randomPattern)
    {
        case 0: StartEruptionPattern(); break;      // 각 함수: 애니메이션 신호 + currentPattern 지정 + StartPattern()
        case 1: StartRazerPattern(); break;
        case 2: StartRampagePattern(); break;
    }
}
```

1. **정리는 공격이 스스로.** `BossManager`는 무엇이 남았는지 모른 채 '정리해라'(`EndPattern`)만 부릅니다. 운석 15개 중 10개째를 만들던 중이어도 남은 작업과 떨어지던 운석이 함께 사라집니다.

### 6.4 공격이 멈추면 약점이 열린다

세 번째 공격이 끝나면 그로기가 시작됩니다. 남은 공격을 정리하고 약점을 켠 뒤, 5초 뒤에 저절로 풀립니다.

```csharp
private void StartGroggy()
{
    if (isGroggy) return;
    isGroggy = true;
    patternCount = 0;

    if (currentPattern != null)
    {
        currentPattern.EndPattern();
        currentPattern = null;
    }

    ForceIdleAnimation();                           // (3) 남은 공격 신호를 지우고 바로 서 있는 동작으로
    bossHeart.gameObject.SetActive(true);           // 약점 열기
    Invoke(nameof(EndGroggy), 5f);                  // 5초 뒤 자동 해제
}
```

2. **그로기 중에는 공격 시계가 멈춥니다.** 그로기가 끝나면 다시 10초를 세므로 6.1의 '대기 10초'가 생깁니다.

3. **애니메이션 신호는 쓰일 때까지 남습니다.** 그로기 직전에 걸린 공격 신호(트리거)가 남아 있으면, 서 있는 동작으로 돌아오자마자 공격 동작이 나옵니다. 그래서 신호 3개를 지우고 서 있는 동작을 바로 재생합니다.

약점은 그로기 때만 켜지고, 위에서 밟을 때만 인정합니다.

```csharp
// BossHeart.cs
private void OnTriggerEnter2D(Collider2D collision)
{
    if (collision.gameObject.name != "Shifter") return;
    if (collision.transform.position.y <= transform.position.y) return;   // 옆·아래에서 닿으면 무시

    boss.CurrentBossCount--;
    bossManager.DamageToBoss();                     // 캐릭터 HP +30, 보스 체력이 0이면 사망 연출
    gameObject.SetActive(false);                    // 그로기 한 번에 한 번만
}
```

![첫 그로기(입장 후 40초). 보스 앞 발판에서 2단 점프로 약점을 밟자 보스 체력 바가 한 칸 줄어듭니다(3 → 2)](images/boss_groggy.gif)

보스전에서 따로 막아 둔 경우는 아래와 같습니다.

| 상황 | 처리 |
|---|---|
| 보스방 입구를 다시 지나감 | 이미 시작했으면 무시 |
| 아무것도 맞히지 못한 총알 | 10초 뒤 스스로 사라짐 |
| 그로기 중 사망 | 5초 뒤로 예약한 그로기 해제(`EndGroggy`)를 취소 |

---

## 7. 캐릭터 이동과 카메라

발판이 켜지고 꺼지는 게임에서는 점프가 늘 같은 높이·같은 거리로 나가야 플레이어가 '색 판단'에만 집중할 수 있습니다. 카메라는 떨어질 곳을 먼저 보여 줘 그 판단을 돕습니다.

### 7.1 점프는 늘 같은 높이로

캐릭터 이동은 강의에서 배운 `CharacterController2D`를 썼습니다. 이 코드는 키네마틱 이동, 즉 엔진의 자동 물리(중력·밀림)에 맡기지 않고 코드가 매번 위치를 직접 정하는 방식입니다. 힘·마찰로 움직이지 않아, 키를 떼면 바로 서고 점프 높이가 늘 같습니다.

쓰다 보니 점프 높이와 거리가 늘 같아 발판 간격을 맞추기 쉬웠고, 그때 이 게임에 맞는 방식이라는 것을 알게 됐습니다.

```csharp
// CharacterController2D.cs (발췌) — 입력과 밖에서 민 양을 합친 뒤, 가로·세로를 따로 재서 막힌 거리까지만 옮긴다
private void FixedUpdate()
{
    IsGrounded = CheckGrounded();
    ApplyGravity();
    ApplyJump();

    Vector3 desireMove =
        new Vector3(moveInput * speed, VerticalVelocity) * Time.fixedDeltaTime
        + externalDelta;                                        // 크러셔 벽이 민 양도 여기서 합쳐진다
    externalDelta = Vector3.zero;

    Vector3 moveVector = Vector3.zero;
    moveVector.x = ResolveAxisMovement(desireMove.x, Vector3.right);
    moveVector.y = ResolveAxisMovement(desireMove.y, Vector3.up);

    if (moveVector.y != desireMove.y) VerticalVelocity = 0.0f;  // 바닥·천장에 막히면 세로 속도 0

    rBody.MovePosition(transform.position + moveVector);
}
```

- 크러셔 벽이 미는 양(`externalDelta`)도 키 입력과 합쳐져 같은 검사를 거칩니다. 3장 원칙 2의 실체가 이 함수입니다.

```csharp
private float ResolveAxisMovement(float move, Vector3 axis)
{
    if (move == 0.0f) return 0.0f;

    Vector3 dir = (move > 0.0f ? axis : -axis);
    float distance = Mathf.Abs(move);
    Bounds box = GetCastBox();                                  // 콜라이더보다 살짝 작은 상자

    RaycastHit2D hit = Physics2D.BoxCast(
        box.center, box.size, 0.0f, dir, distance, collisionLayer);
    if (hit.collider == null) return move;

    // (가로로 막혔을 때 박스 밀기 — 8장)

    float allowed = Mathf.Max(0, hit.distance - skinWidth);     // 이미 겹쳐 있으면 0 → 얼어붙음 (5.2)
    return Mathf.Sign(move) * allowed;
}
```

- 움직이기 전에 캐릭터 크기의 상자를 가려는 방향으로 밀어 보고(BoxCast), 부딪히기 직전까지만 움직입니다. 가로와 세로를 따로 재므로 벽에 붙어도 떨어지고, 바닥 위에서도 걷습니다.
- 수치: 속도 8, 중력 −24, 점프력 11, 2단 점프.

### 7.2 왼쪽을 보면 닿는 위치가 어긋나던 문제

- **문제.** 왼쪽으로 이동하면 캐릭터 그림은 뒤집히는데, 벽·발판에 닿는 위치는 오른쪽을 볼 때 그대로라 그림과 달랐습니다. 발판 왼쪽 끝에서는 그림이 허공에 나가 있는데도 서 있고, 오른쪽 끝에서는 그림이 발판 위에 있는데도 떨어졌습니다.
- **원인.** 좌우 반전을 그림이 붙은 자식 오브젝트에만 했습니다. 콜라이더는 부모(`Shifter`)에 있어 뒤집히지 않았고, 이 콜라이더가 중심에서 오른쪽으로 0.265 치우쳐 있어 차이가 그대로 드러났습니다.
- **해결.** 부모째 좌우로 뒤집어 그림과 콜라이더가 함께 뒤집히게 했습니다.

```csharp
// ShifterController.cs
// 전: animator.transform.localScale = new Vector3(FacingDirection, 1, 1);   // 그림(자식)만 뒤집힘
transform.localScale = new Vector3(FacingDirection, 1, 1);                    // 부모째 뒤집어 콜라이더도 함께
```

- **결과.** 플레이로 확인했습니다. 왼쪽을 보고 벽·발판에 붙어도 그림과 닿는 위치가 맞습니다.

### 7.3 카메라: 위는 넓게, 아래는 좁게

캐릭터가 조금 움직일 때마다 화면이 흔들리지 않도록, 카메라가 따라가지 않는 구역(데드존)을 두고 캐릭터가 그 밖으로 나간 만큼만 따라갑니다.

```csharp
// CameraController.cs (발췌)
void LateUpdate()
{
    if (player == null) return;

    float deltaX = player.position.x - targetX;
    if (Mathf.Abs(deltaX) > horizontalDeadZone)
    {
        targetX += deltaX - (Mathf.Sign(deltaX) * horizontalDeadZone);   // 좌우: ±4
    }

    float deltaY = player.position.y - targetY;
    if (deltaY > 0.0f)
    {
        if (deltaY > deadZoneUp) targetY += deltaY - deadZoneUp;          // 위: 넓게 (5)
    }
    else
    {
        if (Mathf.Abs(deltaY) > deadZoneDown) targetY += deltaY + deadZoneDown;   // 아래: 좁게 (1)
    }

    Vector3 targetPosition = new Vector3(targetX, targetY, 0f) + offset;
    transform.position = Vector3.Lerp(transform.position, targetPosition, smoothing * Time.deltaTime);
}
```

- 점프 한 번은 약 2.5유닛, 2단 점프도 약 5유닛이라(점프력 11·중력 −24로 계산) **제자리 점프로는 카메라가 거의 움직이지 않습니다.** 반대로 떨어질 때는 1유닛만 벗어나도 따라 내려가 착지할 곳을 먼저 보여 줍니다.
- 보스방은 위쪽 구역을 3.5로 줄여, 2단 점프로 약점을 밟으러 올라가면 카메라가 따라 올라갑니다.

![Scene 뷰에 그린 데드존(초록 상자). 위쪽이 넓고 아래쪽이 좁으며, 캐릭터가 이 상자를 벗어날 때만 카메라가 움직입니다](images/15_camera_deadzone.png)

따라가기만 하면 점프마다 화면이 흔들렸습니다. Cinemachine을 쓸까도 고민했지만, 카메라를 여러 대 쓰지도 않고 특수한 촬영 기법도 필요 없다고 판단해 데드존을 직접 만들었습니다. 데드존 상자를 에디터 화면에 그리게 해(`OnDrawGizmos`), 위 5·아래 1을 눈으로 보며 맞췄습니다.

---

## 8. 나머지 기능

깊게 다룬 기능 외에는 대안을 따로 고민하지 않아 한 줄씩 정리합니다.

| 기능 | 구현 요점 | 클래스 |
|---|---|---|
| 레드 타일 | 부모 하나가 자식 타일 전체를 '닿아 있나'(`IsTouching`)로 확인해, 닿아 있는 동안 0.5초마다 HP −5 (보스방은 1초마다 −10). 색과 무관하게 항상 단단하고 항상 아픔 | `Red` |
| 총알 발사기 | 1초마다 아래에서 위로 흰·검 무작위 총알(피해 5, 속도 7.5). 사거리는 시작점 → 끝점 거리라, 레벨에서 끝점만 옮기면 됨 | `BulletLauncher`, `Bullet` |
| 전구 퍼즐 | 버튼 5개가 각자 이웃 전구까지 초록 ↔ 흰색으로 뒤집고, 5개가 모두 초록이면 문 2가 열림. 꼬이면 리셋 버튼 | `ButtontoLight`, `ButtonReset` |
| 박스 운반 (STAGE 1) | 박스를 밀어 골에 떨어뜨리면 박스가 사라지고 문 1이 열림. 밀기는 배운 이동 코드에 직접 추가해, 가로로 막은 것이 Dynamic Rigidbody면 캐릭터 속도를 넘겨 밂. 총알이 박스를 밀어 올리는 동작은 → [9장](#9-돌아보며) | `CharacterController2D`, `BoxGoal` |
| 움직이는 발판 | 시작 위치 기준 위아래 ±2유닛 왕복 | `MovingPlatform` |
| HP | 모든 피해를 한 입구(`ITakeDamage.TakeDamage`)로 받고, HP 값 하나가 범위(0\~100)·HP 바·게임오버를 처리. 맞으면 0.1초 간격으로 3번 빨갛게 깜빡임 | `Shifter`, `ITakeDamage` |
| 화면 색 표시 | 직전 색을 기억해, 색이 바뀐 순간에만 아이콘을 바꿈 | `UIModManager` |
| 씬 전환 | UI 버튼이 글자 값(`Exit`·`Restart`·`Menu`·씬 이름)으로 씬을 바꾸고, 포탈은 W·↑로 다음 씬 | `SceneChanger`, `Portal` |
| 일시정지 · 게임오버 | Esc로 게임 시간을 멈추고 일시정지 화면을 켬. HP가 0이면 게임오버 화면 | `ShifterController`, `Shifter` |
| 보스 UI | 보스방 입구가 보스 이름·체력·타이머 UI를 켬. 타이머는 입장부터 격파까지(분:초) | `OnBosspattern`, `BossHealthUI`, `BossTimerUI` |

| 메뉴 | 게임오버 | 일시정지 |
|---|---|---|
| ![메뉴 화면. 조작법(HOW TO PLAY)과 START · Exit Game 버튼](images/03_menu.png) | ![게임오버 화면. 지금 스테이지를 처음부터 다시 하는 RESTART 버튼과 메뉴로 돌아가는 MENU 버튼](images/04_gameover.png) | ![일시정지 화면. 지금 스테이지를 처음부터 다시 하는 Restart 버튼과 메뉴로 돌아가는 Main 버튼](images/05_pause.png) |

---

## 9. 돌아보며

### 목표한 경험은 어떻게 됐나

수업 시연·발표에서 다른 사람에게 보여 주고 받은 반응은 네 가지였습니다.

| 받은 반응 | 연결되는 부분 |
|---|---|
| 크러셔 벽에서 알맞은 타이밍에 Shift하기가 어렵다 | 벽 간격 1.2초 − 쿨타임 1초로, 연달아 다른 색 벽이 오면 여유가 0.2초뿐입니다(5.1, 값으로 계산). 혼자 플레이하며 맞춘 수치였습니다 |
| 총알이 박스를 밀어 올리는 것이 버그인 줄 알았다 | 만든 뒤 발견해 타이밍 요소로 남긴 동작이었지만(아래 GIF) 플레이어에게 의도가 전달되지 않았습니다 |
| 피격, 보스 약점 공격 같은 피드백이 부족하다 | 맞았을 때는 0.1초씩 3번 깜빡이는 것뿐이고, STAGE 1 총알은 아예 맞지 않습니다 |
| BGM · 효과음이 없어 전체적으로 심심하다 | 소리를 하나도 넣지 않았습니다 |

![아래에서 올라오는 총알이 박스를 밀어 올리는 장면. 만든 뒤 발견해 타이밍 요소로 남긴 동작이다](images/box.gif)

Shift 한 번에 발판·총알·퍼즐이 함께 바뀌는 규칙의 크기는 만들었습니다. 그러나 이 게임을 만든 이유가 '입력에 비해 돌아오는 변화의 크기'였는데, 정작 받은 반응은 피드백이 부족하다는 것이었습니다. 규칙이 크게 바뀌는 것과, 플레이어가 그 변화를 크게 **느끼는** 것은 다른 일이었습니다. 소리·타격감·연출 없이 규칙만으로는 목표한 경험에 절반만 닿았다고 봅니다.

### AI와 함께 만든 방식

AI(Claude)는 제작 중과 제작 뒤 두 시기에 썼습니다.

| 시기 | AI에게 맡긴 것 | 내가 직접 판단한 것 |
|---|---|---|
| 제작 중 | 보스 공격 코드와 리팩토링(코드 정리) 등 코드 작성, 코드 검수(안 쓰는 코드·디버그 로그 정리, `CLEANUP_LOG.md`에 기록) | 게임 규칙(같은 색이면 실체, 크러셔 벽의 반대 규칙, 조준 중 Shift 허용, 그로기와 약점), 보스 공격 3종의 내용과 '시작·정리' 인터페이스 구조, 플레이해 보며 정한 수치(쿨타임, 크러셔 벽, 보스 공격) |
| 제작 뒤 | 프로젝트 문서 초안, 문서의 코드·수치를 실제 소스·씬 값과 대조, 에디터 자동 테스트와 플레이 GIF 촬영 | 문서에 쓸 설계 의도와 고민의 내용, 찾은 동작이 버그인지 의도인지 |

**확인 방법.** AI가 바꾼 코드는 바뀐 부분을 직접 읽고 이해한 뒤 반영했고, 에디터에서 플레이해 동작을 확인했습니다. 빌드 에러 재수정은 실제 빌드로 확인했습니다.

**잘 된 것.**
- 검수에서 안 쓰는 코드와 디버그 로그를 정리하면서, 빌드 에러(아래 '안 된 것')라는 실제 버그도 함께 찾았습니다.
- 에디터 자동 테스트에서 혼자 플레이할 때 지나쳤던 문제를 찾았습니다. STAGE 1 총알 피격 없음, STAGE 2 레드 타일 피해 없음, 보스 레이저가 바로 사라짐, STAGE 2 시작 위치가 테스트용으로 남은 것입니다(→ 아래 [남은 문제](#남은-문제)).

**안 된 것.**
- 문서 초안에서 AI가 코드만 보고 설계 의도를 추정해 적었습니다. '왜 이렇게 했는지'는 코드에 남아 있지 않아, 문장마다 맞는지 하나씩 답하며 확인해야 했습니다.
- 첫 문서가 1,400줄을 넘기고 용어가 많아져, 코드를 모르는 사람이 읽기 어려웠습니다. 결국 질문에 답하며 처음부터 다시 썼습니다(지금 이 문서).
- AI와 한 코드 검수에서 고친 빌드 에러(메뉴 종료 버튼의 에디터 전용 코드)가, 그 뒤 AI와 한 리팩토링 커밋에서 다시 빠졌습니다. 에디터에서는 정상으로 돌아가 드러나지 않았고, 문서를 쓰며 커밋 기록을 다시 읽다가 찾아 고친 뒤 실제 빌드로 확인했습니다.

### 구조를 먼저 정했어야 했다

**① 색을 태그 하나로 정한 대가가 곳곳에 퍼졌습니다.** 색을 태그로 정하자 비교는 한 줄로 끝났지만(4.2), 캐릭터를 이름 `"Shifter"`로 찾는 곳이 6개 파일 7곳으로 늘었습니다. 크러셔 벽은 '`Rigidbody2D`가 있으면 발판이 아니다'라는 코드 어디에도 적히지 않은 규칙에 기댔고(5.3), Shift 쿨타임은 코드 기본값 2초 · 프리팹 1초로 값이 흩어졌습니다. 기능을 더할 때마다 다른 기능의 사정을 알아야 하는 구조였습니다. 여기서 **데이터(색·수치)와 기능이 붙어 있으면, 기능을 하나 고칠 때 엉뚱한 곳이 함께 흔들린다**는 것을 알게 됐습니다. 다음 프로젝트에서는 데이터와 기능을 나누고 코드 사이의 의존을 줄여, 고치기 쉽고 늘리기 쉬운 구조로 시작하겠습니다.

**② AI에게 맡긴 코드는 구조를 먼저 정해 줘야 했습니다.** 보스 공격과 리팩토링 코드를 AI와 함께 쓰면서, 검수에서 고친 빌드 에러가 리팩토링에서 되살아났고(위 '안 된 것'), `BossManager`는 인터페이스를 두고도 공격의 실제 타입을 `switch`로 알게 됐습니다(6.2). 코드를 나누는 원칙(3장)은 만들기 전에 세워 두었지만, 보스 공격을 AI에게 맡길 때 그 원칙을 말해 주지 않았습니다. AI는 맡긴 범위 안에서는 잘 짜지만, **무엇을 지켜야 하는지는 내가 먼저 말하지 않으면 지켜지지 않는다**는 것을 알게 됐습니다. 다음에는 정해 둔 규칙과 구조(어디가 무엇을 알아도 되는지)를 AI에게 먼저 전한 뒤 맡기겠습니다.

### 남은 문제

에디터 자동 테스트·빌드 테스트·촬영에서 찾았고, 아직 고치지 않았습니다.

| 문제 | 원인 | 고칠 방법 |
|---|---|---|
| STAGE 1 총알이 캐릭터를 맞히지 못함 | 총알의 콜라이더가 트리거가 아니라, 엔진 물리를 쓰지 않는 캐릭터와 닿아도 '닿았다' 알림이 오지 않음. 박스를 띄우는 연출(위 박스 GIF)도 같은 설정에서 나옴 | 총알을 트리거로 바꾸고, 박스를 띄우는 동작은 따로 만듦 |
| STAGE 2 레드 타일에서 HP가 깎이지 않음 | STAGE 2의 레드 타일만 씬에서 트리거 설정이 꺼져 있어 '닿아 있나'(`IsTouching`)가 늘 '아니오' | 발판 프리팹 자체를 트리거로 바꿔 씬마다 다른 설정을 없앰 |
| 보스 레이저가 쏘자마자 사라짐 | 레이저의 맞는 범위가 6×4유닛으로 보이는 것보다 커서, 캐릭터가 보스 앞 회색 발판 근처에 있으면 쏘자마자 회색 지형에 닿아 사라짐 (보스 앞에서는 사라지고, 떨어진 자리에서는 날아가는 것을 확인) | 맞는 범위를 보이는 크기로 줄임 |
| 레이저 퍼즐이 흰색이면 직선 한 번에 풀림 | 흰색이 되면 검은 발판이 꺼져 센서까지 직선이 열림 — 배치 실수 (4.4) | 직선 경로를 회색 지형으로 막음 |
| 다른 색 발판 안에서 Shift하면 갇힘 | 다시 켜진 발판과 겹치면 움직일 수 없음 (5.2와 같은 원인) | Shift 전에 겹침을 검사함. 꺼진 콜라이더는 겹침 검사에 잡히지 않으므로 그림 범위로 검사 |
| STAGE 2가 레이저 퍼즐 뒤에서 시작함 | 크러셔 벽을 시험하려고 옮긴 캐릭터 시작 위치(x=85.8)가 커밋에 그대로 남아, 빌드에서 STAGE 2 앞부분을 건너뜀 | 시작 위치를 스테이지 입구로 되돌림 |
| 게임오버 화면에서 Esc를 두 번 누르면 게임이 다시 흐름 | 게임 시간을 게임오버(`Shifter`)와 일시정지(`ShifterController`)가 따로 바꿈 | 진행·일시정지·게임오버 상태를 한 곳에서 관리 |
