import { Component, OnInit } from '@angular/core';
import { Title } from '@angular/platform-browser';
import { Meta } from '@angular/platform-browser';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent implements OnInit {
  title = 'Experienced .NET Developer for Hire';

  constructor(private titleService: Title, private metaService: Meta) {}

  ngOnInit() {
    // Set a meaningful page title reflecting your professional identity
    this.titleService.setTitle(this.title);

    // Update meta description with a concise summary of your professional skills and offerings
    this.metaService.updateTag({
      name: 'description',
      content: 'Highly skilled .NET Developer with extensive experience in building robust web applications. Available for freelance and full-time opportunities.'
    });

    // Include relevant keywords to improve discoverability
    this.metaService.updateTag({
      name: 'keywords',
      content: '.NET, .NET Developer, Freelance Developer, Software Engineer, ASP.NET, C#, Angular, Web Development, Hiring, Job Search'
    });
  }
}
