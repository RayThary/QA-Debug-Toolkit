# QA Debug Toolkit Update Log

QA Debug Toolkit의 버전별 업데이트 기록입니다.  
README에는 최신 업데이트 요약만 남기고, 이전 버전의 변경 내역은 이 문서에서 관리합니다.

---

## v1.4.1 - Screenshot Gallery Cleanup

Screenshot Gallery를 포트폴리오 제출용 마감 상태로 정리한 업데이트입니다.  
기존 Issue Screenshot Attach 기능에 더해, Gallery를 단순 조회용으로도 사용할 수 있도록 Browse / Attach 모드를 분리하고, 저장된 PNG 파일 삭제 흐름을 추가했습니다.

### 추가된 내용

* Screenshot Gallery Browse / Attach 모드 분리

  * Attach Mode: Issue에 스크린샷을 연결하기 위한 모드
  * Browse Mode: 저장된 스크린샷을 확인하고 삭제하기 위한 모드
* Overlay에서 Gallery를 직접 열 수 있는 버튼 구조 추가
* Gallery 썸네일 DeleteButton을 통한 실제 PNG 파일 삭제 기능 추가
* 스크린샷 삭제 전 확인창 추가
* 삭제한 스크린샷이 Issue에 연결되어 있을 경우 Screenshot Path 초기화 처리 추가
* Gallery 단독 조회 시 이미지 클릭으로 Issue 연결 또는 창 닫힘이 발생하지 않도록 처리

### 수정된 내용

* IssueWindow 미리보기 클릭 시 Gallery가 Attach Mode로 열리도록 정리
* Overlay Gallery 버튼은 Browse Mode로 열리도록 분리
* 기존 Gallery 열기 흐름을 목적별 메서드로 정리

  * `OpenGalleryForAttach()`
  * `OpenGalleryOnly()`
* QAScreenshotGallery의 초기화 및 버튼 바인딩 구조 정리

  * Window 초기 상태 설정
  * 기본 Preview Sprite 캐싱
  * Button Listener 연결
* Thumbnail DeleteButton 탐색 기준을 `DeleteButton` 이름으로 정리
* 중복 QAScreenshotGallery 컴포넌트로 인해 삭제 확인창 참조가 null로 처리되던 문제 정리

### 업데이트 목적

Screenshot Gallery를 Issue 첨부 전용 기능에서 끝내지 않고, 저장된 스크린샷을 확인하고 관리할 수 있는 독립 기능으로 정리했습니다.

또한 갤러리를 여는 목적을 Attach / Browse로 나누어, Issue에 스크린샷을 연결하는 흐름과 단순히 Gallery를 확인하는 흐름이 서로 섞이지 않도록 개선했습니다.

---

## v1.4 - Issue Screenshot Attach

Issue에 스크린샷을 연결하여 QA 이슈 기록의 재현성과 확인성을 높이기 위한 업데이트입니다.  
기존에는 Screenshot 저장과 Issue 기록이 분리되어 있었지만, v1.4에서는 저장된 PNG 스크린샷을 Issue에 첨부하고 Save / Load / Export 흐름까지 유지되도록 확장했습니다.

### 추가된 내용

* IssueWindow에 스크린샷 미리보기 영역 추가
* 스크린샷 미리보기 클릭 시 Screenshot Gallery 창 열림
* 저장된 PNG 스크린샷을 Gallery에서 Grid 형태로 표시
* Gallery에서 스크린샷 선택 시 현재 Issue에 연결
* 선택된 스크린샷 파일명 표시 기능 추가
* Clear 버튼으로 Issue와 연결된 스크린샷 해제 기능 추가
* Issue 데이터에 `screenshotPath` 추가
* Issue Save / Load 시 `screenshotPath` 유지
* Issue TXT / TSV Export에 Screenshot Path 출력 추가
* Screenshot Gallery에서 현재 저장 폴더와 Unity 기본 저장 폴더의 PNG 조회 지원

### 수정된 내용

* Clear 시 실제 PNG 파일은 삭제하지 않고 Issue 연결만 해제하도록 처리
* Screenshot Preview Sprite 갱신 시 기존 Preview Sprite 정리 처리
* Screenshot Gallery 관련 책임을 QAScreenshotGallery 쪽으로 분리
* QAIssueViewModule이 Gallery 내부 UI를 직접 제어하지 않도록 정리

### 업데이트 목적

QA 이슈를 기록할 때 텍스트 설명만 남기는 것이 아니라, 실제 발생 화면을 함께 연결할 수 있도록 개선했습니다.

이를 통해 Issue 기록의 재현 근거를 강화하고, TXT / TSV Export에서도 연결된 Screenshot Path를 확인할 수 있도록 구성했습니다.

---

## v1.3.1 - Export & Toggle Cleanup

### 변경 내용

* Checklist TSV Export 파일 확장자를 `.tsv`로 정리
* Checklist TSV Export 헤더와 실제 출력 데이터 컬럼 불일치 수정

  * 기존 헤더에 남아 있던 미사용 컬럼 제거
  * 실제 출력 데이터 기준으로 `No / Checklist Id / Title / Status / Scene / Note` 구조로 정리
* QA 창이 열려 있을 때 F1 Overlay 토글이 동작하지 않도록 처리
* InputField / TMP_InputField 입력 중 F1 Overlay 토글 방지 처리

### 수정 목적

* Checklist Export 결과를 Google Sheets에 붙여넣을 때 컬럼 구조가 어긋나지 않도록 정리
* Issue / Checklist 작성 중 F1 입력으로 Overlay가 의도치 않게 열리거나 닫히는 문제 방지
* QA 기록 작성 중 입력 흐름이 끊기지 않도록 사용성 개선


## v1.3 - Checklist Update

QA 테스트 중 확인해야 할 항목을 런타임에서 직접 작성하고 관리할 수 있도록 Checklist 기능을 추가한 업데이트입니다.
기존 Issue 기능이 발견된 문제를 기록하는 버그 리포트 역할이었다면, Checklist는 테스트 전에 확인할 항목을 정리하고 Pass / Fail 상태를 관리하는 용도로 구성했습니다.

### 추가된 내용

* F1 Overlay에 Checklist 버튼 추가
* ChecklistWindow UI 추가
* ChecklistItemWindow UI 추가
* 체크 항목 추가 / 수정 / 삭제 기능 추가
* 체크 항목 상태 관리 기능 추가

  * Not Tested
  * Pass
  * Fail
* 체크리스트 검색 기능 추가
* 체크리스트 상태 필터 기능 추가
* 체크리스트 JSON 저장 / 로드 기능 추가
* 사람이 읽기 좋은 TXT 리포트 Export 기능 추가
* Google Sheets에 붙여넣기 쉬운 TSV Export 기능 추가
* QAReports/Checklist 경로에 체크리스트 결과 저장

### 구현 구조

* QAChecklistManager 추가
* QAChecklistDataModule 추가
* QAChecklistViewModule 추가
* QAChecklistStorageModule 추가
* QAToolkit에 Checklist 저장 경로 생성 기능 추가

### 업데이트 목적

QA Debug Toolkit이 단순히 발생한 이슈를 기록하는 도구에서 끝나지 않고,
테스트 전에 확인해야 할 항목을 정리하고 실행 결과를 관리할 수 있는 QA 보조 툴로 확장되도록 개선했습니다.

이를 통해 테스트 항목을 먼저 Checklist로 관리하고, 문제가 발생한 항목은 Issue로 따로 기록하는 흐름을 구성할 수 있습니다.

---

## v1.2.1 - Korean UI Text Update

한글 UI 문구 적용을 위한 기본 구조를 추가한 패치 업데이트입니다.
TMP Font Asset이 한글을 지원하는 경우, 지정된 UI 문구를 한글로 적용할 수 있도록 처리했습니다.

### 추가된 내용

* TMP Font Asset의 한글 지원 여부 확인 처리 추가
* 한글 지원 폰트 사용 시 QALocalizedText 문구 적용 처리 추가
* 주요 UI 버튼 및 고정 문구 한글화 대응 구조 추가
* QALocalizedText 컴포넌트 추가
* 테스트용 한글 폰트 파일 및 생성 결과물 Git 제외 처리 추가
* QAReports 결과물 폴더 Git 제외 처리 추가
* .gitignore에 Assets/Test 및 Assets/QAReports 경로 추가

### 업데이트 목적

QA Debug Toolkit의 UI 문구를 한글로 표시할 수 있도록 기본 구조를 추가했습니다.
폰트가 한글을 지원하지 않을 경우 한글 문구 적용을 막아, 텍스트 깨짐을 방지할 수 있도록 했습니다.

---

## v1.2 - Issue Filter Update

v1.1에서 추가한 Status / Severity 데이터를 Issue List에서 직접 활용할 수 있도록 필터 기능을 추가한 업데이트입니다.
기존 제목 검색 기능에 상태와 심각도 조건을 함께 적용하여, QA 이슈를 더 빠르게 분류하고 확인할 수 있도록 개선했습니다.

### 추가된 내용

* Issue List에 Status Filter Dropdown 추가
* Issue List에 Severity Filter Dropdown 추가
* 기존 Search InputField와 Status / Severity 필터 동시 적용
* Status Filter의 All 선택 시 상태 조건 무시
* Severity Filter의 All 선택 시 심각도 조건 무시
* 검색어, 상태, 심각도 조건을 모두 만족하는 이슈만 표시
* 필터 변경 시 Issue List 즉시 갱신
* 이슈 생성 / 수정 / 삭제 후 현재 필터 조건에 맞게 리스트 갱신

### 업데이트 목적

QA 과정에서 기록된 이슈를 단순히 목록으로 확인하는 것뿐만 아니라,
상태와 심각도 기준으로 빠르게 분류할 수 있도록 개선했습니다.

이를 통해 Open 상태의 이슈만 확인하거나, High / Critical 이슈만 따로 확인하는 등 실제 QA 관리 흐름에 더 가까운 사용이 가능하도록 했습니다.

---

## v1.1 - Issue Management Update

Issue 기록 기능을 실제 QA 관리 흐름에 더 가깝게 개선한 업데이트입니다.
기존에는 이슈 제목과 설명 중심으로 기록했다면, v1.1에서는 이슈의 처리 상태와 심각도를 함께 관리할 수 있도록 확장했습니다.

### 추가된 내용

* Issue Window에 Status 드롭다운 추가
* Issue Window에 Severity 드롭다운 추가
* Status 기본값을 Open으로 설정
* Severity 기본값을 Medium으로 설정
* QAIssueData에 status / severity 데이터 추가
* 새 이슈 저장 시 Status / Severity 값 저장
* 기존 이슈 수정 시 저장된 Status / Severity 값 복원
* JSON 저장/로드 흐름에 Status / Severity 반영
* TXT 리포트 Export에 Status / Severity 출력 추가
* Sheet용 TXT Export에 Status / Severity 컬럼 추가

### 수정된 내용

* Dropdown 옵션 표시 시 텍스트가 겹치거나 잘리는 문제 수정
* Dropdown 옵션 높이 및 표시 범위 조정

### 업데이트 목적

QA 과정에서 이슈를 단순히 기록하는 것뿐만 아니라,
이슈의 진행 상태와 심각도를 함께 관리할 수 있도록 개선했습니다.

---

## v1.0 - Base Toolkit Release

QA Debug Toolkit의 기본 기능을 구성한 첫 번째 완료 버전입니다.
Unity 런타임 환경에서 QA 테스트 중 필요한 정보 확인, 스크린샷 저장, 이슈 기록 및 Export 흐름을 구현했습니다.

### 추가된 내용

* F1 키를 통한 QA Overlay On / Off
* Runtime Info 표시

  * 현재 Scene
  * Scene Time
  * FPS
  * TimeScale
* Game View Screenshot 저장 기능
* Issue 생성 / 수정 / 삭제 기능
* Issue 제목 중복 방지
* Issue List Search / Filter 기능
* Issue 데이터 JSON 저장 및 로드
* 사람이 읽기 좋은 TXT 리포트 Export
* Google Sheets에 붙여넣기 쉬운 Sheet용 TXT Export
* 공통 메시지 UI 표시
* Unity Package Export
* 외부 프로젝트 Import 테스트

### 업데이트 목적

게임 실행 중 QA 테스트에 필요한 정보를 빠르게 확인하고,
발견한 이슈를 프로젝트 내부에서 바로 기록할 수 있는 기본 툴킷을 만드는 것을 목표로 했습니다.
