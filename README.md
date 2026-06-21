# QA Debug Toolkit

## 프로젝트 개요

Unity 런타임 환경에서 QA 테스트를 보조하기 위해 제작한 디버그 툴킷입니다.
Play 중 `F1` 키로 QA Overlay를 열고 닫을 수 있으며, Runtime Info 확인, Screenshot 저장, Issue 기록, Checklist 관리, JSON 저장/로드, TXT/TSV Export 기능을 구현했습니다.

* 개발 형태: 개인 프로젝트
* 구현 범위: QA Overlay, Runtime Info, Screenshot 저장, Issue 생성/수정/삭제, Checklist 생성/수정/삭제, Search/Filter, Status/Severity 관리, 한글 UI 대응, JSON 저장/로드, TXT/TSV Export
* 사용 기술: Unity 6, C#, UGUI, TextMeshPro, JSON, File I/O

## 주요 기능

* `F1` 키를 통한 QA Overlay On/Off
* 현재 Scene, Scene Time, FPS, TimeScale 표시
* Game View Screenshot 저장
* Issue 생성 / 수정 / 삭제
* Issue 제목 중복 방지
* Issue List Search / Status Filter / Severity Filter
* Issue Status / Severity 설정
* Checklist 생성 / 수정 / 삭제
* Checklist 제목 중복 방지
* Checklist Search / Status Filter
* Checklist 상태 관리

  * Not Tested
  * Pass
  * Fail
* 한글 UI 문구 적용 지원
* Issue / Checklist 데이터 JSON 저장 및 로드
* 사람이 읽기 좋은 TXT 리포트 Export
* Google Sheets에 붙여넣기 쉬운 TSV Export
* 공통 메시지 UI 표시

## Issue Management

Issue 기능은 Play 중 발견한 문제를 QA Overlay 안에서 바로 기록하고 관리하기 위한 기능입니다.

* 새 이슈 생성 / 수정 / 삭제
* 삭제 확인창
* 제목 중복 방지
* 제목 기준 Search
* Status / Severity Filter
* 멀티라인 Description 입력
* JSON 저장 및 로드
* 이슈별 TXT 리포트 Export
* Google Sheets 붙여넣기용 TSV Export

Issue 데이터에는 Title, Description, Status, Severity, Created Time, Updated Time, Scene, Scene Time 정보가 저장됩니다.

## Checklist Management

Checklist 기능은 QA 테스트 전에 확인해야 할 항목을 정리하고, Play 중 테스트 결과를 관리하기 위한 기능입니다.

* 새 체크 항목 생성 / 수정 / 삭제
* 삭제 확인창
* 제목 중복 방지
* 제목 기준 Search
* Status Filter
* Note 입력
* Not Tested / Pass / Fail 상태 관리
* JSON 저장 및 로드
* 체크리스트 TXT 리포트 Export
* Google Sheets 붙여넣기용 TSV Export

Checklist 데이터에는 Title, Note, Status, Created Time, Updated Time, Scene, Scene Time 정보가 저장됩니다.

## Screenshot

Screenshot 기능은 Play 중 현재 Game View 화면을 이미지 파일로 저장하기 위한 기능입니다.

* QA Overlay에서 Screenshot 버튼으로 저장
* 저장 시점 기준 파일명 자동 생성
* Save Settings에서 지정한 경로 기준으로 저장

## Export 구조

Issue 또는 Checklist 창에서 Export 버튼을 누르면 리포트 파일이 생성됩니다.

* TXT 리포트

  * 사람이 읽기 쉬운 상세 리포트 형식입니다.

* TSV Export

  * 전체 목록을 Tab 구분 형식으로 출력합니다.
  * Google Sheets에 복사/붙여넣기하여 표 형태로 정리할 수 있습니다.

TXT 리포트에는 제목, 설명 또는 메모, 상태, 생성/수정 시간, Scene 정보 등이 출력됩니다.
TSV Export 파일은 Tab 구분 형식으로 출력되어 Google Sheets에 복사/붙여넣기 할 수 있습니다.

## 폴더 구조

```text
Assets/QADebugToolkit
├─ Prefabs
├─ Scenes
├─ Scripts
│  ├─ Core
│  ├─ Issue
│  ├─ Checklist
│  └─ Screenshot
└─ README
```

## 실행 방법

1. `Assets/QADebugToolkit/Prefabs` 폴더에서 `QAToolkit` Prefab을 원하는 Scene에 배치합니다.
2. Play 모드에서 `F1` 키를 눌러 QA Overlay를 열고 닫습니다.
3. Runtime Info, Screenshot, Issue, Checklist 기능을 확인합니다.
4. Issue를 생성한 뒤 Save / Export 기능을 테스트합니다.
5. Checklist를 생성한 뒤 상태 변경, Save / Export 기능을 테스트합니다.

데모 확인이 필요한 경우 `QAToolkitDemo` Scene을 실행하여 기본 구성 상태를 바로 확인할 수 있습니다.

## 목적

Unity 클라이언트 개발 및 QA 포트폴리오를 목적으로 제작했습니다.
게임 실행 중 QA 테스트에 필요한 정보 확인, 이슈 기록, 체크리스트 관리, 저장, Export 흐름을 직접 구현하는 데 중점을 두었습니다.

또한 기능 업데이트 후 직접 테스트를 수행하고, 발견한 문제와 개선 내용을 기록하는 방식으로 개발 포트폴리오와 QA 포트폴리오를 함께 구성하는 것을 목표로 했습니다.

## 최신 업데이트

### v1.4.1 - Screenshot Gallery Cleanup

* Screenshot Gallery Browse / Attach 모드 분리
* Overlay에서 Gallery를 직접 열 수 있는 버튼 구조 추가
* IssueWindow 미리보기 클릭 시 Attach 모드로 Gallery 열기
* Browse 모드에서는 이미지 클릭 시 Issue 연결 및 창 닫힘이 발생하지 않도록 처리
* Gallery 썸네일 DeleteButton을 통한 실제 PNG 파일 삭제 기능 추가
* 스크린샷 삭제 전 확인창 표시
* 삭제된 스크린샷이 Issue에 연결되어 있을 경우 Screenshot Path 초기화
* 중복 QAScreenshotGallery 컴포넌트로 인한 참조 오류 정리
* QAScreenshotGallery 초기화 및 버튼 바인딩 구조 정리

### v1.4 - Issue Screenshot Attach

* Issue에 스크린샷 첨부 기능 추가
* IssueWindow에 스크린샷 미리보기 영역 추가
* 스크린샷 미리보기 클릭 시 Screenshot Gallery 창 열림
* 저장된 PNG 스크린샷을 Gallery에서 Grid 형태로 표시
* Gallery에서 스크린샷 선택 시 현재 Issue에 연결
* 선택된 스크린샷 파일명 표시 기능 추가
* Clear 버튼으로 Issue와 연결된 스크린샷 해제 기능 추가
* Clear 시 실제 PNG 파일은 삭제하지 않고 Issue 연결만 해제
* Issue Save / Load 시 `screenshotPath` 유지
* Issue TXT / TSV Export에 Screenshot Path 출력 추가
* Screenshot Gallery에서 현재 저장 폴더와 Unity 기본 저장 폴더의 PNG 조회 지원

이전 버전 및 전체 업데이트 기록은 `UpdateLog.md`에서 관리합니다.
