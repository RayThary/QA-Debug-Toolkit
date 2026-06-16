# QA Debug Toolkit Update Log

QA Debug Toolkit의 버전별 업데이트 기록입니다.
README에는 최신 업데이트 요약만 남기고, 이전 버전의 변경 내역은 이 문서에서 관리합니다.

---

## v1.4 - Issue Screenshot Attach

Issue에 스크린샷을 직접 연결할 수 있도록 Screenshot Attach 기능을 추가한 업데이트입니다.
기존 Issue 기록이 텍스트 중심이었다면, v1.4에서는 저장된 스크린샷을 Gallery에서 선택해 Issue와 연결하고,
이후 저장 / 로드 / Export 흐름에서도 스크린샷 경로를 함께 관리할 수 있도록 확장했습니다.

### 추가된 내용

* IssueWindow에 스크린샷 미리보기 영역 추가
* 미리보기 영역 클릭 시 Screenshot Gallery 창 열림
* Screenshot Gallery에서 저장된 PNG 스크린샷 목록 표시
* Grid Layout 기반으로 스크린샷 썸네일 표시
* 스크린샷 선택 시 현재 Issue에 `screenshotPath` 연결
* 선택된 스크린샷 파일명 표시 기능 추가
* Clear 버튼으로 현재 Issue와 연결된 스크린샷 해제 기능 추가
* Clear 시 실제 PNG 파일은 삭제하지 않고 Issue 연결만 해제
* Issue Save / Load 시 `screenshotPath` 유지
* Issue TXT Export에 Screenshot Path 출력 추가
* Issue TSV Export에 Screenshot Path 컬럼 추가


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

---

### v1.4.1 후보

* Screenshot Gallery 기능을 기본 기능으로 분리
* Issue Screenshot Attach 외에도 재사용 가능한 Gallery 구조 정리
* Gallery UI 템플릿 및 기본 설정 정리
* Screenshot Gallery 경로 조회 / 썸네일 생성 로직 안정화
* Issue Screenshot Attach 관련 코드 정리 및 Inspector 연결 구조 보완
