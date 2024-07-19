# Sokoban
> 콘솔 프로젝트로 제작한 소코반게임 입니다

## 프로젝트 정보

- 게임 장르 : 2D 퍼즐
- 제작 인원 : 1 인 (개인 프로젝트)
- 제작 기간 : 2일
- 사용 엔진과 언어 : Visual Studio 2019, C#

## 플레이 방법

```
1. 방향키로 캐릭터를 움직이세요
2. 골인지점(G)에 각 소코반 상자(B)를 밀어주세요
3. 모든 상자를 골인지점에 밀었을 때, 퍼즐이 풀리게 됩니다.
```
# 플레이 화면

## 1. 메인 메뉴

> 게임을 처음 들어갔을 때 보여주는 화면입니다.
>
> 메뉴 조작법과 게임 메뉴가 나타납니다.

![메인메뉴gif](https://github.com/user-attachments/assets/28916f61-0af8-4e50-bb78-d7d420bb38b7)

## 2. 게임 플레이

> 게임 플레이 화면입니다.
>
> 게임 시작으로 들어가 맵을 고르고 플레이를 합니다
>
> 게임을 클리어 하면 메인화면으로 돌아옵니다.

![게임플레이gif](https://github.com/user-attachments/assets/ee40dd3a-a6fb-4740-a5cd-9b3a34dc079e)

## 3. 맵 제작

> 맵 제작을 하는 화면입니다.

### 3-1. 맵 크기 설정 및 에이터 사용

> 맵 크기 설정 및 에이터 사용 화면입니다.
>
> 맵 크기를 입력 받아 제작할 맵을 만듭니다.
>
> 만든 맵을 에디터 화면에 띄어주고 맵을 제작합니다.

![맵제작및에이터사용gif](https://github.com/user-attachments/assets/82b1ba80-30fa-4313-8974-e8be08ae6a54)

### 3-2. 맵 테스팅 및 맵 저장

> 맵 테스팅 및 맵 저장 화면입니다.
>
> 다음과 같은 조건을 만족해야 맵 테스트를 진행할 수 있습니다.
> 1. 플레이어는 **존재** 해야하고 **1명** 이어야 한다.
> 2. 박스와 골인점의 갯수는 **같아야한다.**
>
>
> 테스트를 진행하여 클리어를 하면 그 때 맵을 저장할 수 있습니다.
>
> 저장할 때 제목을 입력 받고 (y/n) 중 하나를 입력합니다
> - y : 확정
> - n : 재작성
>
> 제목 확정을 하면 맵을 만들어서 저장합니다 
>
> 제작한 맵은 맵 리스트에서 **R**키를 눌러 확인할 수 있습니다.

![맵테스트및저장gif](https://github.com/user-attachments/assets/d21d8574-16fd-4ca3-bfe3-36d99f624c5d)

## 4. 게임 종료

> 게임 종료 화면입니다.

![게임종료gif](https://github.com/user-attachments/assets/287695d7-f136-4735-90ff-0c3065b1d981)

