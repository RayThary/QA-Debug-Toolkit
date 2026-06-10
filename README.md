# QA Debug Toolkit

## 프로젝트 개요

Unity 런타임 환경에서 QA 테스트를 보조하기 위해 제작한 디버그 툴킷입니다.
Play 중 `F1` 키로 QA Overlay를 열고 닫을 수 있으며, Runtime Info 확인, Screenshot 저장, Issue 기록 및 Export 기능을 구현했습니다.

* 개발 형태: 개인 프로젝트
* 구현 범위: QA Overlay, Runtime Info, Screenshot 저장, Issue 생성/수정/삭제, Search/Filter, Status/Severity 관리, JSON 저장/로드, TXT/Sheet Export
* 사용 기술: Unity 6, C#, UGUI, TextMeshPro, JSON, File I/O

## 주요 기능

* `F1` 키를 통한 QA Overlay On/Off
* 현재 Scene, Scene Time, FPS, TimeScale 표시
* Game View Screenshot 저장
* Issue 생성 / 수정 / 삭제
* Issue 제목 중복 방지
* Issue List Search / Filter
* Issue Status / Severity 설정
* Issue 데이터 JSON 저장 및 로드
* 사람이 읽기 좋은 TXT 리포트 Export
* Google Sheets에 붙여넣기 쉬운 Sheet용 TXT Export
* 공통 메시지 UI 표시

## Export 구조

Export 버튼을 누르면 두 가지 파일이 함께 생성됩니다.

* 이슈별 TXT 리포트
* 전체 이슈 목록 Sheet용 TXT 파일

TXT 리포트에는 이슈의 제목, 설명, 상태, 심각도, 생성/수정 시간, Scene 정보 등이 출력됩니다.
Sheet용 TXT 파일은 Tab 구분 형식으로 출력되어 Google Sheets에 복사/붙여넣기 할 수 있습니다.

## 폴더 구조

```text
Assets/QADebugToolkit
├─ Prefabs
├─ Scenes
├─ Scripts
│  ├─ Core
│  ├─ Issue
│  └─ Screenshot
└─ README
```

## 실행 방법

1. `Assets/QADebugToolkit/Prefabs` 폴더에서 `QAToolkit` Prefab을 원하는 Scene에 배치합니다.
2. Play 모드에서 `F1` 키를 눌러 QA Overlay를 열고 닫습니다.
3. Runtime Info, Screenshot, Issue 기능을 확인합니다.
4. Issue를 생성한 뒤 Save / Export 기능을 테스트합니다.

데모 확인이 필요한 경우 `QAToolkitDemo` Scene을 실행하여 기본 구성 상태를 바로 확인할 수 있습니다.

## 목적

Unity 클라이언트 개발 및 QA 포트폴리오를 목적으로 제작했습니다.
게임 실행 중 QA 테스트에 필요한 정보 확인, 이슈 기록, 저장, Export 흐름을 직접 구현하는 데 중점을 두었습니다.

또한 기능 업데이트 후 직접 테스트를 수행하고, 발견한 문제와 개선 내용을 기록하는 방식으로 개발 포트폴리오와 QA 포트폴리오를 함께 구성하는 것을 목표로 했습니다.

## 최신 업데이트

### v1.3 - Checklist Update

* QA 테스트 항목을 관리할 수 있는 Checklist 기능 추가
* F1 Overlay에 Checklist 버튼 추가
* ChecklistWindow 및 ChecklistItemWindow UI 추가
* 체크 항목 추가 / 수정 / 삭제 기능 추가
* 체크 항목 상태 관리 추가

  * Not Tested
  * Pass
  * Fail
* 체크리스트 검색 및 상태 필터 기능 추가
* 체크리스트 JSON 저장 / 로드 기능 추가
* TXT / TSV Export 기능 추가
* QAReports/Checklist 경로에 체크리스트 결과 저장

이전 버전 및 전체 업데이트 기록은 `UpdateLog.md`에서 관리합니다.

