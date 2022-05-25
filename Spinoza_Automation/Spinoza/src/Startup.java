import java.util.concurrent.TimeUnit;

import org.openqa.selenium.WebDriver;
import org.openqa.selenium.chrome.ChromeDriver;

public class Startup {

	public static void main(String[] args) throws InterruptedException {
		System.setProperty("webdriver.chrome.driver", ".\\lib\\chromedriver.exe");
		WebDriver driver = new ChromeDriver();
		driver.manage().timeouts().implicitlyWait(15, TimeUnit.SECONDS);

		
		driver.manage().window().maximize();
		driver.get("http://localhost:3000/");
//		driver.get("https://wonderful-bay-0dbd0a003.1.azurestaticapps.net/");
		
		//TEST'S CREATE TEST
		System.out.println("-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-");
		System.out.println("TEST START - TEST'S CREATE");
		System.out.println("-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-");
		
		Test1 t1 = new Test1();
		t1.CreateNewTest(driver); // CREATE TEST AND CHECK TITLE CONFLICT
		Thread.sleep(1000);
		t1.CreateNewTest_Test1(driver); // CREATE TEST WITHOUT DATA
		Thread.sleep(1000);
		t1.CreateNewTest_Test2(driver); // CREATE TEST WITH TITLE ONLY
		Thread.sleep(1000);
		t1.CreateNewTest_Test3(driver); // CREATE TEST WITH ALL DATA FIELDS
		Thread.sleep(1000);
		
		System.out.println("-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-");
		System.out.println("TEST END - TEST'S CREATE");
		System.out.println("-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-");

		Thread.sleep(5000);
		
		//TEST'S EDIT TEST
		System.out.println("-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-");
		System.out.println("TEST START - TEST'S EDIT");
		System.out.println("-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-");
		
		Test2 t2 = new Test2();
		t2.EditTestTitle(driver); // EDIT TEST TITLE
		Thread.sleep(1000);
		t2.EditTestTitle_Test1(driver); // EDIT TEST WITHOUT TITLE
		Thread.sleep(1000);
		t2.EditTestDescription(driver); // EDIT TEST DESCRIPTION
		Thread.sleep(1000);
		t2.EditTestTags(driver); // EDIT TAGS
		Thread.sleep(1000);
		t2.EditTestQuestions_CreateNewQuestion(driver); // EDIT TEST BY CREATE NEW QUESTION
		Thread.sleep(1000);
		t2.EditTestQuestion_ChooseFromCatalog(driver); // EDIT TEST BY CHOSING QUESTION FORM CATALOG
		Thread.sleep(1000);
		
		System.out.println("-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-");
		System.out.println("TEST END - TEST'S EDIT");
		System.out.println("-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-");

		Thread.sleep(5000);
		
		//TEST'S CREATE QUESTION
		System.out.println("-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-");
		System.out.println("TEST START - ADD QUESTION TO TEST");
		System.out.println("-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-");
		
		Test3 t3 = new Test3();
		t3.CreateNewQuestion(driver, 1); // CREATE QUESTION WITH 'TEXT' ANSWERS
		Thread.sleep(1000);
		t3.CreateNewQuestion(driver, 2); // CREATE QUESTION WITH 'MULTIPLE' ANSWERS
		Thread.sleep(1000);
		
		System.out.println("-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-");
		System.out.println("TEST END - ADD QUESTION TO TEST");
		System.out.println("-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-");

		Thread.sleep(5000);
		
		//TEST'S SEARCH TEST BY TAG
		System.out.println("-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-");
		System.out.println("TEST START - SEARCH TEST'S BY TAG");
		System.out.println("-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-");
		
		Test4 t4 = new Test4();
		t4.SearchTestsByTag(driver, "Python");
		Thread.sleep(1000);
		
		System.out.println("-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-");
		System.out.println("TEST END - SEARCH TEST'S BY TAG");
		System.out.println("-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-");

		driver.quit();
	}

}
