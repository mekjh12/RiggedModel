# Animate 클래스 사용법
1. XmlDae 
  - RawModel3d 가져오기
```c#
xmlDae = new XmlDae(EngineLoop.PROJECT_PATH + "\\Res\\guybrush.dae");
```
  - Action 가져오기 (Action 파일의 생성법은 아래 참고)
```c#
xmlDae.AddAction(EngineLoop.PROJECT_PATH + "\\Res\\Action\\Jump.dae")
```
> 다음이다.
`ㅁㅁㅁㅁ`
> [Contribution guidelines for this project](docs/CONTRIBUTING.md)

abc:
```mermaid
graph TD;
    A-->B;
    A-->C;
    B-->D;
    C-->D;
```
<details>

<summary>Tips for collapsed sections</summary>

### You can add a header

You can add text within a collapsed section. 

You can add an image or a code block, too.

```ruby
   puts "Hello World"
```

</details>

2. Entity
3. 
4. AniModel = Entity + XmlDae

# Mixamo 사용법
1. 캐릭터 가져오기
2. Weight 적용하기
3. Action 추가하기
