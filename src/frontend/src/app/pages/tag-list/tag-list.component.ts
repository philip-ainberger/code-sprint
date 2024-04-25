import { Component, OnInit } from '@angular/core';
import { TaggingGrpcServiceClient } from '../../generated/Protos/tagging.client';
import { CreateTagRequest, DeleteTagRequest, ListTagsRequest, Tag } from '../../generated/Protos/tagging';

@Component({
  selector: 'app-tag-list',
  templateUrl: './tag-list.component.html',
})
export class TagListPage implements OnInit {
  tags: Tag[];
  name: string = "";
  selectedTag?: Tag;

  constructor(private service: TaggingGrpcServiceClient) {
    this.tags = [];
  }

  ngOnInit(): void {
    this.fetchData();
  }

  fetchData(): void{
    this.service.listTags(ListTagsRequest.create())
      .then(res => {
        this.tags = res.response.tags;
      },
      err => {
        console.log(err);
      }
    );
  }

  addTag(): void {
    this.service.createTag(CreateTagRequest.create({
      name: this.name
    })).then(res => {
      console.log(res);
      this.fetchData();
      this.name = "";
    },
    err => {
      console.log(err);
    });
  }

  deleteTag(id?: string): void {
    this.service.deleteTag(DeleteTagRequest.create({
      id: id
    })).then(res => {
      console.log(res);
      this.fetchData();
    },
    err => {
      console.log(err);
    });
  }
}
