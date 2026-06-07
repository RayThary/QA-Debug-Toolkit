# QA Debug Toolkit Update Log

QA Debug Toolkit의 버전별 업데이트 기록입니다.
README에는 최신 업데이트 요약만 남기고, 이전 버전의 변경 내역은 이 문서에서 관리합니다.

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

## 다음 업데이트 후보

### v1.3 후보

* UI 고정 문구 한글화
* TMP 한글 폰트 적용 확인
* Screenshot과 Issue 연결 기능
* Log Capture 기능
* Checklist 기능
